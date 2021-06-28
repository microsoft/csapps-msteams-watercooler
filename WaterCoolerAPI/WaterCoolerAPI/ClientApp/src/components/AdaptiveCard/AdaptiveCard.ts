// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export const getInitAdaptiveCard = () => {
  return (
    {
      "type": "AdaptiveCard",
      "body": [
        {
          "type": "Image",
          "spacing": "Default",
          "url": "",
          "size": "Stretch",
          "altText": ""
        },
        {
          "type": "TextBlock",
          "text": "",
          "wrap": true,
          "size": "Large",
          "weight": "Bolder"
        },
        {
          "type": "TextBlock",
          "wrap": true,
          "weight": "Lighter",
          "size": "Large",
          "text": ""
        },
        {
          "type": "TextBlock",
          "text": "",
          "wrap": true
        }
      ],
      "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
      "version": "1.0"
    }
  );
}

export const getCardImageLink = (card: any) => {
  return card.body[0].url;
}

export const setCardImageLink = (card: any, imageLink?: string) => {
  card.body[0].url = imageLink;
}

export const getCardTitle = (card: any) => {
  return card.body[1].text;
}

export const setCardTitle = (card: any, title: string) => {
  card.body[1].text = title;
}

export const getCardSummary = (card: any) => {
  return card.body[2].text;
}

export const setCardSummary = (card: any, summary?: string) => {
  card.body[2].text = summary;
}

export const getCardLink = (card: any) => {
  return card.body[3].text;
}

export const setCardLink = (card: any, link?: string) => {
  card.body[3].text = link;
}