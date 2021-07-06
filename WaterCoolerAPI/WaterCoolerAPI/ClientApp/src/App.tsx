import React, { Suspense } from 'react';
import './App.scss';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import Dashboard from './components/Dashboard/dashboard';
import WelcomeCard from './components/WelcomeCard/WelcomeCard';
import newRoom from './components/NewRoom/NewRoom';
import { TeamsThemeContext, getContext, ThemeStyle } from 'msteams-ui-components-react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Provider, teamsTheme, teamsDarkTheme, teamsHighContrastTheme } from '@fluentui/react-northstar'

export interface IAppState {
  theme: string;
  themeStyle: number;
}

class App extends React.Component<{}, IAppState> {

  constructor(props: {}) {
    super(props);
    this.state = {
      theme: "",
      themeStyle: ThemeStyle.Light,
    }
  }

  public componentDidMount() {
    microsoftTeams.initialize();
    microsoftTeams.getContext((context) => {
      let theme = context.theme || "";
      this.updateTheme(theme);
      this.setState({
        theme: theme
      });
    });

    microsoftTeams.registerOnThemeChangeHandler((theme) => {
      this.updateTheme(theme);
      this.setState({
        theme: theme,
      }, () => {
        this.forceUpdate();
      });
    });
  }

  public setThemeComponent = () => {

    if (this.state.theme === "dark") {
      return (
        <Provider theme={teamsDarkTheme}>
          <div className="darkContainer">
            {this.getAppDom()}
          </div>
        </Provider>
      );
    }
    else if (this.state.theme === "contrast") {
      return (
        <Provider theme={teamsHighContrastTheme}>
          <div className="highContrastContainer">
            {this.getAppDom()}
          </div>
        </Provider>
      );
    } else {
      return (
        <Provider theme={teamsTheme}>
          <div className="defaultContainer">
            {this.getAppDom()}
          </div>
        </Provider>
      );
    }
  }

  private updateTheme = (theme: string) => {
    if (theme === "dark") {
      this.setState({
        themeStyle: ThemeStyle.Dark
      });
    } else if (theme === "contrast") {
      this.setState({
        themeStyle: ThemeStyle.HighContrast
      });
    } else {
      this.setState({
        themeStyle: ThemeStyle.Light
      });
    }
  }

  public getAppDom = () => {
    const context = getContext({
      baseFontSize: 10,
      style: this.state.themeStyle
    });
    return (
      <TeamsThemeContext.Provider value={context}>
        <Suspense fallback={<div></div>}>
          <div className="appContainer">
            <BrowserRouter>
              <Switch>
                <Route exact path="/" component={Dashboard} />
                <Route exact path="/newRoom" component={newRoom} />
                <Route exact path="/welcomeCard" component={WelcomeCard} />
              </Switch>
            </BrowserRouter>
          </div>
        </Suspense>
      </TeamsThemeContext.Provider>
    );
  }

  public render(): JSX.Element {
    return (
      <div>
        {this.setThemeComponent()}
      </div>
    );
  }
}

export default App;