import * as React from 'react';
import { Flex, Input, Button, Loader } from '@fluentui/react-northstar';
import './Newroom.scss';
import { createNewRoom, getUserList, getRoomIcons } from "../../apis/WaterCoolerApi";
import { INewRoomState, IRoomDetails } from './NewRoomInterface';
import * as WConst from '../../WaterCoolerConstants';
import * as microsoftTeams from "@microsoft/teams-js";
import { PeoplePicker } from '@microsoft/mgt-react';

export class Newroom extends React.Component<{}, INewRoomState> {
  constructor(props: {}) {
    super(props);
    this.state = {
      roomTitle: "",
      roomDescription: "",
      roomImage: "",
      roomIcons: [],
      errorRoomTitleMessage: "",
      errorRoomDescriptionMessage: "",
      loader: true,
      loaderMessage: "",
      users: [],
      selectedPeople: []
    }
  }

  public async componentDidMount() {
    microsoftTeams.initialize();
    this.getRoomIcons();
  }

  private onRoomTitleChanged = (event: any) => {
    this.setState({
      roomTitle: event.target.value,
      errorRoomTitleMessage: (50 - event.target.value.length) + " " + WConst.constants.charactersLeft
    });
  }

  private onRoomDescriptionChanged = (event: any) => {
    this.setState({
      roomDescription: event.target.value,
      errorRoomDescriptionMessage: (200 - event.target.value.length) + " " + WConst.constants.charactersLeft
    });
  }
  private onImageClicked = (event: any) => {
    this.setState({
      roomImage: event.target.attributes.src.value
    });
  }

  private onPeopleSearch = (event: any) => {
    this.getUserList(event.target.input.value);
  }

  private onSelectionChanged = (event: any) => {
    this.setState({
      selectedPeople: event.target.selectedPeople
    });
  }

  public render(): JSX.Element {
    let roomsImage = this.state.roomIcons.map((element) =>
      <img src={element.url} title={element.name} className={this.state.roomImage == element.url ? "groupImageSelected" : "groupImage"} onClick={this.onImageClicked} />
    );
    if (this.state.loader) {
      return (
        <div className="loaderContainer">
          <Loader></Loader>
          <p className="loaderMessage">{this.state.loaderMessage}</p>
        </div>
      );
    } else {
      return (
        <div className="taskModule">
          <Flex column className="formContainer" vAlign="stretch" gap="gap.small" styles={{ background: "white" }}>
            <Flex className="scrollableContent">
              <Flex column className="formContentContainer">
                <p className="fieldError">{this.state.errorRoomTitleMessage}</p>
                <Input className="inputField"
                  label={WConst.constants.roomTitle}
                  autoComplete="off"
                  onChange={this.onRoomTitleChanged}
                  fluid
                  maxLength={50}
                />
                <p className="fieldError">{this.state.errorRoomDescriptionMessage}</p>
                <Input className="inputField"
                  label={WConst.constants.shortDescription}
                  autoComplete="off"
                  onChange={this.onRoomDescriptionChanged}
                  fluid
                  maxLength={200}
                />
                <label>{WConst.constants.peoplePickerLabel}</label>
                <PeoplePicker
                  people={this.state.users}
                  onKeyUp={this.onPeopleSearch}
                  placeholder={WConst.constants.peoplePickerPlaceholder}
                  show-max="5"
                  selectionChanged={this.onSelectionChanged}
                  selected-person={this.state.selectedPeople}
                  disabled={this.state.selectedPeople.length > 4}
                  />
                <label className="fieldLabel">{WConst.constants.chooseRoomImage}</label>
                <div className="groupImageContainer">{roomsImage}</div>
              </Flex>
              <Flex className="footerContainer" vAlign="end" hAlign="end">
                <Flex className="buttonContainer" gap="gap.small">
                  <Flex.Item push>
                    <Button content="Cancel" secondary id="cancelBtn" onClick={this.onCancelRoom} />
                  </Flex.Item>
                  <Button content={WConst.constants.create} id="createBtn" disabled={!this.state.roomTitle || !this.state.roomDescription} onClick={this.onCreateRoom} primary />
                </Flex>
              </Flex>
            </Flex>
          </Flex>
        </div>
      )
    }
  }

  private onCreateRoom = () => {
    this.setState({
      loader: true,
      loaderMessage: WConst.constants.roomLoadingMessage
    });
    const roomData: IRoomDetails = {
      name: this.state.roomTitle,
      description: this.state.roomDescription,
      logoUrl: this.state.roomImage,
      selectedPeople: this.state.selectedPeople
    };
    createNewRoom(roomData).then(onlineMeetingURL => {
      microsoftTeams.executeDeepLink(onlineMeetingURL);
    });
  }

  private onCancelRoom = () => {
    microsoftTeams.tasks.submitTask();
  }

  private getUserList = (userSearchKey: string) => {
    getUserList(userSearchKey).then(usersList => {
      this.setState({
        users: usersList.value
      })
    })
  }

  private getRoomIcons = () => {
    getRoomIcons().then(roomIcons => {
      this.setState({
        loader: false,
        roomIcons: roomIcons,
        roomImage: roomIcons[0].url
      })
    })
  }
}

export default Newroom
