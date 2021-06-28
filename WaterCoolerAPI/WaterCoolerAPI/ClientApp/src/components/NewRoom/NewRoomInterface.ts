export interface INewRoomState {
  roomTitle: string,
  roomDescription: string,
  roomImage: string,
  roomIcons: IRoomIcons[],
  errorRoomTitleMessage: string,
  errorRoomDescriptionMessage: string,
  loader: boolean,
  loaderMessage: string,
  users: IUsers[],
  selectedPeople: IUsers[]
}

export interface IRoomDetails {
  name: string,
  description: string,
  logoUrl: string,
  selectedPeople: IUsers[]
}

export interface IUsers {
  displayName: string,
  givenName: string,
  id: string,
  imAddress: string,
  jobTitle: string,
  surname: string,
  userPrincipalName: string
}

export interface IRoomIcons {
  name: string,
  url: string
}