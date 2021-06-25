// <copyright file="RoomDataController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;
    using WaterCoolerAPI.Bot;
    using WaterCoolerAPI.Common;
    using WaterCoolerAPI.Helpers;
    using WaterCoolerAPI.Interface;
    using WaterCoolerAPI.Meetings.ParticipantData;
    using WaterCoolerAPI.Models;
    using WaterCoolerAPI.Repositories;
    using WaterCoolerAPI.Repositories.RoomData;
    using WaterCoolerAPI.Repositories.RoomLogoUrlData;
    using WaterCoolerAPI.Repositories.UserLoginData;

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
        private readonly IUserLoginDataRepository userLoginDataRepository;
        private readonly IDistributedCache cache;
        private readonly TableRowKeyGenerator tableRowKeyGenerator;
        private readonly ILogger<RoomDataController> logger;
        private readonly IGraph graph;
        private readonly CreateRoomHelper createRoomHelper;
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
            IRoomLogoDataRepository roomLogoDataRepository,
            IDistributedCache cache,
            IUserLoginDataRepository userLoginDataRepository,
            CreateRoomHelper createRoomHelper)
        {
            this.roomDataRepository = roomDataRepository;
            this.tableRowKeyGenerator = tableRowKeyGenerator;
            this.logger = logger;
            this.graph = graph;
            this.httpContextAccessor = httpContextAccessor;
            this.bot = bot;
            this.activeRoomAndParticipantHelper = activeRoomAndParticipantHelper;
            this.roomLogoDataRepository = roomLogoDataRepository;
            this.cache = cache;
            this.userLoginDataRepository = userLoginDataRepository;
            this.createRoomHelper = createRoomHelper;
        }

        /// <summary>
        /// Returns the Active rooms List with active participants.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route(HttpRouteConstants.GetActiveRoomsWithParticipants)]
        public async Task<List<ActiveRoomData>> GetActiveParticipantListAsync()
        {
            string activeRoomData = this.cache.GetString(Common.Constants.ActiveRoomData);
            return JsonConvert.DeserializeObject<List<ActiveRoomData>>(activeRoomData);
        }

        /// <summary>
        /// Checks if the user has logged in for the first time or not.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route(HttpRouteConstants.CheckUserLogin)]
        public async Task<UserData> CheckUserLogin()
        {
            Task<UserPrincipalData> userData = this.createRoomHelper.GetUserPrincipalAndId();
            Task<UserData> newUser = this.userLoginDataRepository.CheckFirstLoginAsync(userData.Result);
            return newUser.Result;
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
            Task<UserPrincipalData> userData = this.createRoomHelper.GetUserPrincipalAndId();

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
                        roomDataEntity.UserPrincipleName = userData.Result.UserPrincipalName;
                        roomDataEntity.ObjectId = userData.Result.ObjectId;

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
