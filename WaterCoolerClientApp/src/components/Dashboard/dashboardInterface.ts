export interface dashboardState {
  loader: boolean,
  loaderMessage: string,
  roomsList: IRoomDetails[],
  filteredRoom: IRoomDetails[],
  roomSearchString: string
}

export interface IRoomDetails {
  name: string,
  description: string,
  logoUrl: string,
  startDateTime: string,
  endDateTime: string,
  meetingUrl: string,
  userPrincipleName: string,
  objectId: string,
  callId: string,
  isActive: boolean,
  partitionKey: string,
  rowKey: string,
  timestamp: string,
  eTag?: string,
  userList?: IUsers[]
}

export interface IUsers {
  userId: string,
  displayName: string,
  displayPicture: string
}

export interface ITaskinfo {
  url: string,
  title: string,
  height: number,
  width: number,
  fallbackUrl: string
}