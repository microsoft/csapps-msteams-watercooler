import React, { Component } from 'react';
import { IRoomsState, IRoomDetails } from "./RoomsInterface";
import { Button } from "@fluentui/react-northstar";
import * as WConst from '../../WaterCoolerConstants';
import * as microsoftTeams from "@microsoft/teams-js";
import './Rooms.scss';
import Timer from '../Timer/Timer';
import UserDisplayPicture from '../UserDisplayPicture/UserDisplayPicture';
import {user} from '../../apis/axiosJWTDecorator';

export interface IRoomsProps {
  roomsData: IRoomDetails[]
}

class Rooms extends React.Component<IRoomsProps, IRoomsState> {
  constructor(props: IRoomsProps) {
    super(props);
  }

  private joinMeeting = (meetingUrl: string) => {
    microsoftTeams.executeDeepLink(meetingUrl);
  }

  render() {
    let roomData = this.props.roomsData.map((element, i) =>
      <div className="cornerCuts cornerCutsRooms" key={element.startDateTime}>
        <div className="innerCard">
          <p className="heading" title={element.name}>{element.name}</p>
          <Button 
            content={WConst.constants.join}
            className="joinButton"
            primary
            size="small"
            onClick={() => this.joinMeeting(element.meetingUrl)}
            disabled={element.userList?.some((users) => users.userId === user.oid)}></Button>
          <img src={element.logoUrl} className="avatar" />
          <div className="avatarGroupRooms">
            {element.userList?.map((elem, index) => {
              if (index < 6) {
                return <UserDisplayPicture userDetails={elem} key={elem.userId}/>
              }
            })}
            {element.userList ? element.userList.length > 6 ? <div className="moreParticipants">+{element.userList?.length - 6}</div> : '' : ''}
          </div>
          <p className="roomDesc" title={element.description}>{element.description}</p>
          <Timer startDateTime={element.startDateTime}/>
        </div>
      </div>);
    return (
      <div>{roomData}</div>
    )
  }
}

export default Rooms
