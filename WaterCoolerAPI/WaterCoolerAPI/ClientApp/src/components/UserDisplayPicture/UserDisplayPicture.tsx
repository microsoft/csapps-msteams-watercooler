import React, { Component } from 'react'
import { fetchMsGraphProfilePic } from "../../apis/WaterCoolerApi"

export interface IUserDisplayPictureState {
  displayPicture: any
}

export interface IUserDisplayPictureProps {
  userDetails: {
    userId: string,
    displayName: string
  }
}

export class UserDisplayPicture extends React.Component<IUserDisplayPictureProps, IUserDisplayPictureState> {
  constructor(props: IUserDisplayPictureProps) {
    super(props);
    this.state = {
      displayPicture: ""
    }
    this.getDisplayPicture();
  }
  
  private getDisplayPicture = () => {
    fetchMsGraphProfilePic(this.props.userDetails.userId).then(blobImage => {
      var reader = new FileReader();
      if(blobImage) {
        reader.readAsDataURL(blobImage);
        reader.onloadend = () => {
          var profileBase64 = reader.result;
          this.setState({
            displayPicture: profileBase64
          })
        };
      }
    });
  }

  render() {
    return (
      this.state.displayPicture ? <img src={this.state.displayPicture} className="avatarGroupRoomImage" title={this.props.userDetails.displayName} /> : <div className="moreParticipants" title={this.props.userDetails.displayName}>{this.props.userDetails.displayName.match(/\b(\w)/g)?.join('')}</div>
    )
  }
}

export default UserDisplayPicture
