import React from 'react';
import dotnetify from 'dotnetify';
import { RouteLink } from 'dotnetify/dist/dotnetify-react.router';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import MenuItem from 'material-ui/MenuItem';
import RaisedButton from 'material-ui/RaisedButton';
import SelectField from 'material-ui/SelectField';
import InfoBox from '../components/forecast/InfoBox';
import CoinHistory from '../components/forecast/CoinHistory';
import ForecastHistory from '../components/forecast/ForecastHistory';
import { grey200, grey400, pink100, pink400, orange100, orange400, blue200, purple100, purple400 } from 'material-ui/styles/colors';
import BasePage from '../components/BasePage';
import InlineInfo from '../components/form/InlineInfo';
import ThemeDefault from '../styles/theme-default';
import Avatar from 'material-ui/Avatar';
import Paper from 'material-ui/Paper';
import ContentRemoveCircle from 'material-ui/svg-icons/content/remove-circle';
import NavigationArrowDropDown from 'material-ui/svg-icons/navigation/arrow-drop-down';
import NavigationArrowDropUp from 'material-ui/svg-icons/navigation/arrow-drop-up';
import style from 'material-ui/svg-icons/image/style';

class ForecastPage extends React.Component {

  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect("Forecast", this);
    this.dispatch = state => this.vm.$dispatch(state);
    this.routeTo = route => this.vm.$routeTo(route);

    this.state = {
      Coins: [],
      Handle: '',
      Count: '',
      StartingValue: '',
      Worth: '',
      CurrentValue: '',
      Delta: '',
      HistoricalGraph: '',
      ForecastGraph: '',
      NearForecastGraph: '',
      FarForecastGraph: '',
      ForecastData: '',
      NearForecastData: '',
      FarForecastData: ''
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    let { Coins, Id, Handle, Count, StartingValue, Worth, CurrentValue, Delta, HistoricalGraph, ForecastGraph, NearForecastGraph, FarForecastGraph, ForecastData, NearForecastData, FarForecastData } = this.state;

    const styles = {
        selectLabel: { color: pink400 },
        form: {
            padding: '10px 20px',
            paddingBottom: 15,
            marginBottom: 10
        },
      toggleDiv: {
        maxWidth: 300,
        marginTop: 40,
        marginBottom: 5
      },
      toggleLabel: {
        color: grey400,
        fontWeight: 100
      },
      buttons: {
        marginTop: 30,
        float: 'right'
      }, 
      trendDiv: {
        height: 150
        },
      infoBox: {
          padding: '10px 20px',
        },
      saveButton: { marginLeft: 5 }
    };

    const handleSelectFieldChange = (event, idx, value) => this.routeTo(Coins.find(i => i.Id === value).Route);
      
    return (
      <MuiThemeProvider muiTheme={ThemeDefault}>
        <BasePage title="Forecasting" navigation="HistoCoin / Forecasting">
          <div className="container-fluid">
            <div className="row">
              <div style={styles.form} className="col-xs-12 col-sm-12 col-md-5 col-lg-5 m-b-15 ">
                <SelectField
                  value={Id}
                  onChange={handleSelectFieldChange}
                  floatingLabelText="Select coin for details"
                  floatingLabelStyle={styles.selectLabel}
                >
                  {Coins.map(item =>
                    <MenuItem key={item.Id} value={item.Id} primaryText={`${item.Handle} (${item.Count})`} />
                  )}
                </SelectField>
              </div>
            </div>

            <div className="row">
                <div className="col-xs-12 col-sm-12 col-md-5 col-lg-5 m-b-15 " style={styles.infoBox}>
                    <InfoBox
                        icon={null}
                        color={null}
                        title="Coin Handle"
                        value={Handle} />

                    <InfoBox
                        icon={null}
                        color={null}
                        title="Current wallet count"
                        value={Count} />

                    <InfoBox
                        icon={null}
                        color={null}
                        title="Cost on purchase (USD)"
                        value={StartingValue} />
                </div>
                    
            <div className="col-xs-12 col-sm-12 col-md-5 col-lg-5 m-b-15 " style={styles.infoBox}>
                <InfoBox
                      icon={null}
                      color={null}
                      title="Current Value (USD)"
                      value={`$${this.state.CurrentValue}`}
                    />

                    <InfoBox
                        icon={null}
                        color={null}
                        title="Worth (USD)"
                        value={`$${this.state.Worth}`}
                    />

                    <InfoBox
                        icon={null}
                        color={null}
                        title="Delta"
                        value={`$ ${this.state.Delta > 0 ? `+${this.state.Delta}` : this.state.Delta}`}
                    />
                </div>
            </div>

            <div className="row">
                <div className="col-xs-12 col-sm-12 col-md-12 col-lg-12 col-md m-b-15">
                <CoinHistory
                    title="Value History"
                    data={this.state.HistoricalGraph.Values}
                    dates={this.state.HistoricalGraph.Labels}
                    color={grey200} />
                </div>
            </div>

            <div className="container-fluid">
                <div className="row">
                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                        <InfoBox style={styles.trendDiv}
                            icon={null}
                            color={purple100}
                            title="Daily Change"
                            value={<InlineInfo
                            leftValue=
                            {this.state.DailyChange === 0 || this.state.DailyChange === null
                                ? <Avatar icon={<ContentRemoveCircle />} />
                                : (this.state.ForecastData.DailyChange > 0
                                    ? <Avatar icon={<NavigationArrowDropUp />} />
                                    : <Avatar icon={<NavigationArrowDropDown />} />)}
                            rightValue=
                              {`$ ${this.state.ForecastData.DailyChange > 0 ? `+${this.state.ForecastData.DailyChange}` : this.state.ForecastData.DailyChange}`} />}
                        />

                        <InfoBox
                            icon={null}
                            color={purple100}
                            title="Forecast Value"
                            value={`$${this.state.ForecastData.ForecastValue}`}
                        />
                    </div>

                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                        <InfoBox style={styles.trendDiv}
                            icon={null}
                            color={purple100}
                            title="Daily Trend"
                            value={<InlineInfo
                              leftValue=
                                {this.state.ForecastData.Trend === 0 || this.state.ForecastData.Trend === null
                                ? <Avatar icon={<ContentRemoveCircle />} />
                                : (this.state.ForecastData.Trend > 0
                                    ? <Avatar icon={<NavigationArrowDropUp />} />
                                    : <Avatar icon={<NavigationArrowDropDown />} />)}
                            rightValue={`${this.state.ForecastData.Trend > 0 ? `+${this.state.ForecastData.Trend}` : this.state.ForecastData.Trend} %`} />}
                        />

                  <InfoBox
                      icon={null}
                      color={purple100}
                      title="Forecast Worth"
                      value={`$${this.state.ForecastData.ForecastWorth}`}
                  />
                </div>

                        <div className="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-md m-b-15">
                            <CoinHistory
                                title="Linear Forecast"
                                data={this.state.ForecastGraph.Values}
                                dates={this.state.ForecastGraph.Labels}
                                color={grey200} />
                        </div>
            </div>

            <div className="row">
                <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                    <InfoBox style={styles.trendDiv}
                        icon={null}
                        color={orange100}
                        title="Eager Daily Change"
                        value={<InlineInfo
                          leftValue=
                            {this.state.NearForecastData.DailyChange === 0 || this.state.NearForecastData.DailyChange === null
                                ? <Avatar icon={<ContentRemoveCircle />} />
                                : (this.state.NearForecastData.DailyChange > 0
                                    ? <Avatar icon={<NavigationArrowDropUp />} />
                                    : <Avatar icon={<NavigationArrowDropDown />} />)}
                            rightValue=
                              {`$ ${this.state.NearForecastData.DailyChange > 0 ? `+${this.state.NearForecastData.DailyChange}` : this.state.NearForecastData.DailyChange}`} />}
                    />

                    <InfoBox
                        icon={null}
                        color={orange100}
                        title="Eager Forecast Value"
                        value={`$${this.state.NearForecastData.ForecastValue}`}
                    />
                </div>

                <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                    <InfoBox style={styles.trendDiv}
                        icon={null}
                        color={orange100}
                        title="Eager Daily Trend"
                        value={<InlineInfo
                          leftValue=
                            {this.state.NearForecastData.Trend === 0 || this.state.NearForecastData.Trend === null
                                ? <Avatar icon={<ContentRemoveCircle />} />
                                : (this.state.NearForecastData.Trend > 0
                                    ? <Avatar icon={<NavigationArrowDropUp />} />
                                    : <Avatar icon={<NavigationArrowDropDown />} />)}
                            rightValue=
                              {`${this.state.NearForecastData.Trend > 0 ? `+${this.state.NearForecastData.Trend}` : this.state.NearForecastData.Trend} %`} />}
                    />

                    <InfoBox
                        icon={null}
                        color={orange100}
                        title="Eager Forecast Worth"
                        value={`$${this.state.NearForecastData.ForecastWorth}`}
                    />
                </div>

                    <div className="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-md m-b-15">
                        <CoinHistory
                            title="Eager Forecast"
                            data={this.state.NearForecastGraph.Values}
                            dates={this.state.NearForecastGraph.Labels}
                            color={grey200} />
                    </div>
              </div>

                <div className="row">
                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                        <InfoBox style={styles.trendDiv}
                            icon={null}
                            color={pink100}
                            title="Skeptical Daily Change"
                            value={<InlineInfo
                              leftValue=
                                {this.state.FarForecastData.DailyChange === 0 || this.state.FarForecastData.DailyChange === null
                                    ? <Avatar icon={<ContentRemoveCircle />} />
                                    : (this.state.FarForecastData.DailyChange > 0
                                        ? <Avatar icon={<NavigationArrowDropUp />} />
                                        : <Avatar icon={<NavigationArrowDropDown />} />)}
                                rightValue=
                                  {`$ ${this.state.FarForecastData.DailyChange > 0 ? `+${this.state.FarForecastData.DailyChange}` : this.state.FarForecastData.DailyChange}`} />}
                        />

                        <InfoBox
                            icon={null}
                            color={pink100}
                            title="Skeptical Forecast Value"
                            value={`$${this.state.FarForecastData.ForecastValue}`}
                        />
                    </div>

                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                        <InfoBox style={styles.trendDiv}
                            icon={null}
                            color={pink100}
                            title="Skeptical Daily Trend"
                            value={<InlineInfo
                              leftValue=
                                {this.state.FarForecastData.Trend === 0 || this.state.FarForecastData.Trend === null
                                    ? <Avatar icon={<ContentRemoveCircle />} />
                                    : (this.state.FarForecastData.Trend > 0
                                        ? <Avatar icon={<NavigationArrowDropUp />} />
                                        : <Avatar icon={<NavigationArrowDropDown />} />)}
                                rightValue=
                                  {`${this.state.FarForecastData.Trend > 0 ? `+${this.state.FarForecastData.Trend}` : this.state.FarForecastData.Trend} %`} />}
                        />

                        <InfoBox
                            icon={null}
                            color={pink100}
                            title="Skeptical Forecast Worth"
                            value={`$${this.state.FarForecastData.ForecastWorth}`}
                        />
                    </div>

                        <div className="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-md m-b-15">
                            <CoinHistory
                                title="Skeptical Forecast"
                                data={this.state.FarForecastGraph.Values}
                                dates={this.state.FarForecastGraph.Labels}
                                color={grey200} />
                        </div>
                </div>

                    <div className="row">
                        <div className="col-xs-12 col-sm-12 col-md-12 col-lg-12 col-md m-b-15">
                            <ForecastHistory
                                dataSet0={this.state.ForecastGraph.Values}
                                dataSet1={this.state.NearForecastGraph.Values}
                                dataSet2={this.state.FarForecastGraph.Values}
                                labelsSet={this.state.ForecastGraph.Labels}
                                color0={purple400} 
                                color1={orange400}
                                color2={pink400} />
                        </div>
                    </div>
            </div>
        </div>
      </BasePage>
    </MuiThemeProvider>
    );
  }
}

export default ForecastPage;
