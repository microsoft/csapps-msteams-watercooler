import React from 'react';
import { getRoomList, getWelcomeStatus } from "../../apis/WaterCoolerApi";
import Rooms from '../Rooms/Rooms';
import './dashboard.scss';
import { dashboardState, ITaskinfo } from './dashboardInterface';
import * as WConst from '../../WaterCoolerConstants';
import { Loader } from '@fluentui/react-northstar';
import { Input, Button } from '@fluentui/react-northstar';
import * as microsoftTeams from "@microsoft/teams-js";
import { initializeIcons } from '@fluentui/font-icons-mdl2';
import { Icon } from '@fluentui/react/lib/Icon';
import RoomPlaceholder from '../../resources/roomPlaceholder.png'
import UserPlaceholder from '../../resources/userPlaceholder.png'
import FemaleUserPlaceholder from '../../resources/femaleUserPlaceholder.png'
import FemaleUserPlaceholderTwo from '../../resources/femaleUserPlaceholderTwo.png'

initializeIcons();

class dashboard extends React.Component<{}, dashboardState> {
  private interval: any;
  constructor(props: {}) {
    super(props);
    this.state = {
      loader: true,
      loaderMessage: WConst.constants.dashboardLoadingMessage,
      roomsList: [],
      filteredRoom: [],
      roomSearchString: ''
    }
  }

  componentDidMount() {
    console.log('dashboard loads');
    microsoftTeams.initialize();
    this.showWelcomeCard();
    this.getRoomList();
    this.interval = setInterval(() => {
      this.getRoomList();
    }, 8000);
  }

  // open modal for new room screen
  showModal = () => {
    let taskInfo: ITaskinfo = {
      url: `${window.location.origin}\\newRoom`,
      title: WConst.constants.newRoom,
      height: 570,
      width: 610,
      fallbackUrl: `${window.location.origin}\\newRoom`,
    }

    let submitHandler = (err: any, result: any) => {
      this.getRoomList();
    };
    microsoftTeams.tasks.startTask(taskInfo, submitHandler);
  };

  filterRoom = (event: any) => {
    this.setState({
      roomSearchString: event.target.value,
      filteredRoom: this.state.roomsList.filter(rooms => rooms.name.toLowerCase().includes(event.target.value))
    });
  }

  render() {
    if (this.state.loader) {
      return (
        <div className="loaderContainer">
          <Loader></Loader>
          <p className="loaderMessage">{this.state.loaderMessage}</p>
        </div>
      );
    } else {
      const userPlaceholder: string[] = [UserPlaceholder, FemaleUserPlaceholder, FemaleUserPlaceholderTwo];
      let userImage = userPlaceholder.map((element) =>
        <img src={element} className="avatarGroupImage" key={element} alt=""/>
      );
      return (
        <div>
          <div className="searchBar">
            <Input className="inputField"
              placeholder={WConst.constants.find}
              autoComplete="off"
              onChange={(event) => {
                event.persist();
                this.filterRoom(event);
              }}
              fluid
            />
            <Icon iconName="Search" className="searchIcon" />
          </div>
          <div className="mainContainer">
            <div className="cornerCuts">
              <div className="innerCard">
                <p className="heading">{WConst.constants.createRoom}</p>
                <img src={RoomPlaceholder} className="avatar" alt="User Icons" />
                <div className="avatarGroup">{userImage}</div>
                <Button className="createRoomBtn" primary onClick={this.showModal}><Icon iconName="PeopleAdd" />&ensp;{WConst.constants.createRoom}</Button>
              </div>
            </div>
            <Rooms roomsData={this.state.roomSearchString === '' ? this.state.roomsList : this.state.filteredRoom} />
          </div>
        </div>
      );
    }
  }

  // get rooms data
  private getRoomList = () => {
    getRoomList().then(roomList => {
      this.setState({
        roomsList: roomList,
        loader: false
      })
    })
  }

  // show welcome card for the first time user
  private showWelcomeCard = () => {
    getWelcomeStatus().then(welcomeCardData => {
      if (welcomeCardData.isFirstLogin) {
        let taskInfo: ITaskinfo = {
          url: `${window.location.origin}\\welcomeCard`,
          title: WConst.constants.appTour,
          height: 500,
          width: 613,
          fallbackUrl: `${window.location.origin}\\welcomeCard`
        }
        microsoftTeams.tasks.startTask(taskInfo);
      }
    });
  }

  componentWillUnmount() {
    clearInterval(this.interval);
  }
}
export default dashboard
