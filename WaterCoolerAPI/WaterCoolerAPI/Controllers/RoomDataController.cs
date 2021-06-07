// <copyright file="RoomDataController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Controllers
{
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;
    using WaterCoolerAPI.Bot;
    using WaterCoolerAPI.Common;
    using WaterCoolerAPI.Interface;
    using WaterCoolerAPI.Meetings.ParticipantData;
    using WaterCoolerAPI.Repositories;
    using WaterCoolerAPI.Repositories.RoomData;
    using WaterCoolerAPI.Repositories.RoomLogoUrlData;

    /// <summary>
    /// Controller for room data.
    /// </summary>
    [Route(HttpRouteConstants.ApiControllerRoute)]
    [ApiController]
    [Authorize]
    [EnableCors(Constants.CORSAllowAllPolicy)]
    public class RoomDataController : ControllerBase
    {
        private readonly IRoomDataRepository roomDataRepository;
        private readonly TableRowKeyGenerator tableRowKeyGenerator;
        private readonly ILogger<RoomDataController> logger;
        private readonly IGraph graph;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Bot bot;
        private readonly IActiveRoomAndParticipant activeRoomAndParticipantHelper;
        private readonly IRoomLogoDataRepository roomLogoDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomDataController"/> class.
        /// </summary>
        /// <param name="roomDataRepository">IRoomDataRepository instance.</param>
        /// <param name="tableRowKeyGenerator">TableRowKeyGenerator instance.</param>
        /// <param name="logger">ILogger instance.</param>
        /// <param name="graph">IGraph instance.</param>
        /// <param name="httpContextAccessor">IHttpContextAccessor instance.</param>
        /// <param name="bot">Bot instance.</param>
        /// <param name="activeRoomAndParticipantHelper">ActiveRoomAndParticipantHelper instance.</param>
        /// <param name="roomLogoDataRepository">RoomLogoDataRepository instance.</param>
        public RoomDataController(
            IRoomDataRepository roomDataRepository,
            TableRowKeyGenerator tableRowKeyGenerator,
            ILogger<RoomDataController> logger,
            IGraph graph,
            IHttpContextAccessor httpContextAccessor,
            Bot bot,
            IActiveRoomAndParticipant activeRoomAndParticipantHelper,
            IRoomLogoDataRepository roomLogoDataRepository)
        {
            this.roomDataRepository = roomDataRepository;
            this.tableRowKeyGenerator = tableRowKeyGenerator;
            this.logger = logger;
            this.graph = graph;
            this.httpContextAccessor = httpContextAccessor;
            this.bot = bot;
            this.activeRoomAndParticipantHelper = activeRoomAndParticipantHelper;
            this.roomLogoDataRepository = roomLogoDataRepository;
        }

        /// <summary>
        /// Returns the active rooms list.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route(HttpRouteConstants.GetActiveRoomList)]
        public async Task<IEnumerable<RoomDataEntity>> GetRoomsListAsync()
        {
            var result = await this.roomDataRepository.GetActiveRoomsAsync();
            return result;
        }

        /// <summary>
        /// Returns the Active Participants List with User details.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route(HttpRouteConstants.GetActiveParticipantList)]
        public async Task<List<ActiveRoomData>> GetActiveParticipantListAsync()
        {
            List<ActiveRoomData> activeRoomList = await this.activeRoomAndParticipantHelper.ActiveParticipantListAsync();
            return activeRoomList;
        }

        /// <summary>
        /// Returns the logo Url with its name.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route(HttpRouteConstants.GetRoomLogoUrls)]
        public async Task<List<BlobLogoData>> GetRoomLogoUrls()
        {
            List<BlobLogoData> blobLogoData = await this.roomLogoDataRepository.GetLogoUrlsAsync();
            return blobLogoData;
        }

        /// <summary>
        /// Create a meeting for the new room and store the information into storage table.
        /// </summary>
        /// <param name="roomDataEntity">RoomDataEntity instance.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route(HttpRouteConstants.CreateRoom)]
        public async Task<string> CreateRoomAsync([FromBody] RoomDataEntity roomDataEntity)
        {
            string userPrincipleName = null;
            string objectId = null;
            var httpContext = this.httpContextAccessor.HttpContext;
            httpContext.Request.Headers.TryGetValue(Constants.Authorization, out StringValues assertion);
            var idToken = assertion.ToString().Split(" ")[1];
            if (idToken.Length > 0)
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.ReadToken(idToken) is JwtSecurityToken tokenS)
                {
                    userPrincipleName = tokenS.Claims.Where(a => a.Type.Equals(Constants.UpnKey)).Select(b => b).FirstOrDefault()?.Value;
                    objectId = tokenS.Claims.Where(a => a.Type.Equals(Constants.OidKey)).Select(b => b).FirstOrDefault()?.Value;
                }
            }

            string onlineMeetingUrl = null;
            try
            {
                if (roomDataEntity != null)
                {
                    roomDataEntity.PartitionKey = RoomDataTableNames.RoomDataPartition;
                    roomDataEntity.RowKey = this.tableRowKeyGenerator.CreateNewKeyOrderingOldestToMostRecent();
                    roomDataEntity.SelectedPeopleContent = JsonConvert.SerializeObject(roomDataEntity.SelectedPeople);

                    var graphServiceClient = await this.graph.GetGraphServiceClient();
                    var onlineMeeting = await this.graph.CreateOnlineMeeting(graphServiceClient, roomDataEntity);
                    if (onlineMeeting != null)
                    {
                        onlineMeetingUrl = onlineMeeting.JoinWebUrl;

                        roomDataEntity.MeetingUrl = onlineMeeting.JoinWebUrl;
                        roomDataEntity.StartDateTime = onlineMeeting.StartDateTime;
                        roomDataEntity.EndDateTime = onlineMeeting.EndDateTime;
                        roomDataEntity.UserPrincipleName = userPrincipleName;
                        roomDataEntity.ObjectId = objectId;

                        var callId = (await this.bot.JoinTeamsMeetingAsync(roomDataEntity)).Id;
                        if (!string.IsNullOrEmpty(callId))
                        {
                            roomDataEntity.CallId = callId;
                            roomDataEntity.IsActive = true;
                        }

                        await this.roomDataRepository.CreateOrUpdateAsync(roomDataEntity);
                        this.logger.LogInformation(Constants.RoomDataCreateOrUpdateSucessful);
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }

            return onlineMeetingUrl;
        }

        /// <summary>
        /// Retreives application access token.
        /// </summary>
        /// <returns>Application acess token.</returns>
        [HttpGet]
        [Route(HttpRouteConstants.Token)]
        public async Task<string> GetAccessToken()
        {
            return await this.graph.GetApplicationAccessToken();
        }
    }
}
