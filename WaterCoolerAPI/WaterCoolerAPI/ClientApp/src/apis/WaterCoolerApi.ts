import axios, { user } from './axiosJWTDecorator';
import { getBaseUrl, graphAPIUrl } from '../configVariables';
import { IRoomDetails } from "../components/Rooms/RoomsInterface";
import { IRoomIcons } from '../components/NewRoom/NewRoomInterface';

export interface IGraphResponse {
  value: []
}

export interface IWelcomeCardResponse {
  isFirstLogin: boolean,
  termsofUseText: string,
  termsofUseUrl: string,
}

let baseAxiosUrl = getBaseUrl() + '/api';

export const getRoomList = async (): Promise<IRoomDetails[]> => {
  let url = baseAxiosUrl + "/RoomData/GetActiveRoomsWithParticipants";
  const roomList = await axios.get(url);
  return roomList.data;
}

export const createNewRoom = async (payload: {}): Promise<string> => {
  let url = baseAxiosUrl + "/RoomData/CreateRoom";
  const onlineMeetingURL = await axios.post(url, payload);
  return onlineMeetingURL.data;
}

export const getUserList = async (payload: {}): Promise<IGraphResponse> => {
  let url = baseAxiosUrl + "/RoomData/token";
  const applicationAccessToken = await axios.get(url);

  let graphUrl = graphAPIUrl+ "/users/" + user.oid + "/people/?$search=" + payload;

  return fetch(graphUrl, {
    headers: { "Authorization": `Bearer ${applicationAccessToken.data}`, "X-PeopleQuery-QuerySources": `Mailbox,Directory` }
  }).then(response => response.json());
}

export const getRoomIcons = async (): Promise<IRoomIcons[]> => {
  let url = baseAxiosUrl + "/RoomData/GetRoomLogoUrls";
  const roomIcons = await axios.get(url);
  return roomIcons.data;
}

export const getWelcomeStatus = async (): Promise<IWelcomeCardResponse> => {
  let url = baseAxiosUrl + "/RoomData/CheckUserLogin";
  const welcomeStatus = await axios.get(url);
  return welcomeStatus.data;
}

export const fetchMsGraphProfilePic = async (userId: string) => {
  let url = baseAxiosUrl + "/RoomData/token";
  const applicationAccessToken = await axios.get(url);

  let graphUrl = graphAPIUrl + "/users/" + userId + "/photo/$value";

  const requestHeaders = { Authorization: `Bearer ${applicationAccessToken.data}`, "Content-Type": "blob" };
  const requestOptions = { method: "GET", headers: requestHeaders };
  return await fetch(graphUrl, requestOptions).then(function (response) {
    if (!response.ok) {
      throw response;
    } else {
      return response.blob();
    }
  }).catch(function (err) {
    throw err
  });
};