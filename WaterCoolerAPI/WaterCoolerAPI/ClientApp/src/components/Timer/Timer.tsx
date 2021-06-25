import React, { Component } from 'react'

export interface ITimerState {
  timerData: string
}
export interface ITimerProps {
  startDateTime: string
}
export class Timer extends React.Component<ITimerProps, ITimerState> {
  private interval: any;
  constructor(props: ITimerProps) {
    super(props);
    this.state = {
      timerData: ''
    }
    this.interval = setInterval(() => {
      this.getTimer(props.startDateTime);
    }, 1000);
  }

  private getTimer = (startDateTime: string) => {
    let startStamp = new Date(startDateTime).getTime();
    let newStamp = new Date().getTime()
    let diff = Math.round((newStamp - startStamp) / 1000);

    let d = Math.floor(diff / (24 * 60 * 60));
    diff = diff - (d * 24 * 60 * 60);
    let h = Math.floor(diff / (60 * 60));
    diff = diff - (h * 60 * 60);
    let m = Math.floor(diff / (60));
    diff = diff - (m * 60);
    let s = diff;
    this.setState({
      timerData: h > 0 ? `${h}:${m}:${s}s` : `${m}:${s}s`
    })
  }

  render() {
    return (
      <p className="callDuration">{this.state.timerData}</p>
    )
  }

  componentWillUnmount() {
    clearInterval(this.interval);
  }
}

export default Timer
