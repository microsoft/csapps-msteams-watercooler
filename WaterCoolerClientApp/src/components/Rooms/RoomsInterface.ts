export interface IRoomsState {}

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
  displayName: string,
  givenName: string,
  jobTitle: string,
  officeLocation: string,
  userPrincipalName: string,
  displayPicture?: string
}