import React from 'react';
import { IWelcomeCardInterface, IWelcomeCardProps } from './IWelcomeCardInterface';
import * as AdaptiveCards from "adaptivecards";
import MarkdownIt from "markdown-it";
import { getInitAdaptiveCard, setCardTitle, setCardImageLink, setCardSummary, setCardLink } from '../AdaptiveCard/AdaptiveCard';
import { Button } from '@fluentui/react-northstar';
import "./WelcomeCard.scss";
import * as WConst from '../../WaterCoolerConstants';
import * as microsoftTeams from "@microsoft/teams-js";
import { getWelcomeStatus } from "../../apis/WaterCoolerApi";

AdaptiveCards.AdaptiveCard.onProcessMarkdown = function (text, result) {
  result.outputHtml = new MarkdownIt().render(text);
  result.didProcess = true;
}

export class WelcomeCard extends React.Component<IWelcomeCardProps, IWelcomeCardInterface> {
  private card: any;
  private step: number = 0;
  constructor(props: IWelcomeCardProps) {
    super(props);
    this.card = getInitAdaptiveCard();
    this.state = {
      card: this.card,
      tourStep: 0,
      termsOfUse: ''
    }
  }

  componentDidMount() {
    microsoftTeams.initialize();
    this.getWelcomeCardLink();
    let adaptiveCard = new AdaptiveCards.AdaptiveCard();
    adaptiveCard.parse(this.state.card);
    let renderedCard = adaptiveCard.render();
    if(renderedCard) {
      document.getElementsByClassName('adaptiveCardContainer')[0].appendChild(renderedCard);
    }
  }

  render() {
    return (
      <div>
        <div className="adaptiveCardContainer"></div>
        {this.state.tourStep !== (WConst.constants.welcomeCardContent.length - 1) ? <Button content="Next" primary size="medium" className="steps" onClick={this.onNext} /> : <Button content="Finish" primary size="medium" className="steps" onClick={this.onFinish} /> }
      </div>
    )
  }

  public setWelcomeCard = (card: any) => {
    setCardImageLink(card, WConst.constants.welcomeCardContent[this.step].image);
    setCardTitle(card, WConst.constants.welcomeCardContent[this.step].title);
    setCardSummary(card, WConst.constants.welcomeCardContent[this.step].summary);
    this.step === 0 ? setCardLink(card, this.state.termsOfUse) : setCardLink(card, '');
  }

  private onNext = () => {
    this.step +=  1;
    this.setState({
      tourStep: this.step
    })
    this.setWelcomeCard(this.card);
    this.setState({
      card: this.card
    }, () => {
      this.updateCard();
    });
  }

  private onFinish = () => {
    microsoftTeams.tasks.submitTask();
  }

  private updateCard = () => {
    const adaptiveCard = new AdaptiveCards.AdaptiveCard();
    adaptiveCard.parse(this.state.card);
    const renderedCard = adaptiveCard.render();
    const container = document.getElementsByClassName('adaptiveCardContainer')[0].firstChild;
    if(renderedCard) {
      if (container != null) {
        container.replaceWith(renderedCard);
      } else {
        document.getElementsByClassName('adaptiveCardContainer')[0].appendChild(renderedCard);
      }
    }
  }

  private getWelcomeCardLink = () => {
    getWelcomeStatus().then(welcomeCardData => {
      this.setState({
        termsOfUse: `[${welcomeCardData.termsofUseText}](${welcomeCardData.termsofUseUrl})`
      }, () => {
        this.setWelcomeCard(this.card);
        this.updateCard();
      })
    });
  }
}

export default WelcomeCard
