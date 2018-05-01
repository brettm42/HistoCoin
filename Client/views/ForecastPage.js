import React from 'react';
import dotnetify from 'dotnetify';
import { RouteLink } from 'dotnetify/dist/dotnetify-react.router';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import MenuItem from 'material-ui/MenuItem';
import SelectField from 'material-ui/SelectField';
import InfoBox from '../components/forecast/InfoBox';
import CoinHistory from '../components/forecast/CoinHistory';
import ForecastHistory from '../components/forecast/ForecastHistory';
import StackedTile from '../components/forecast/StackedTile';
import { grey200, grey400, pink400, pink500, pink600, orange500, orange600, cyan500, cyan600, blue200, blue400, purple500, purple600, purple400 } from 'material-ui/styles/colors';
import BasePage from '../components/BasePage';
import ThemeDefault from '../styles/theme-default';
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
    let { Coins, Id, Handle, Count } = this.state;

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
            padding: '15px 0 25px 15px'
        },
        saveButton: { marginLeft: 5 },
        forecastRow0: { titleColor: purple600, color: purple500 },
        forecastRow1: { titleColor: orange600, color: orange500 },
        forecastRow2: { titleColor: cyan600, color: cyan500 }
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
                <div className="col-xs-12 col-sm-12 col-md-6 col-lg-3 m-b-15 " style={styles.infoBox}>
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
                        title="Cost on purchase"
                        value={`$${this.state.StartingValue}`} />
                </div>
                    
            <div className="col-xs-12 col-sm-12 col-md-6 col-lg-3 m-b-15 " style={styles.infoBox}>
                <InfoBox
                      icon={null}
                      color={null}
                      title="Current Value"
                      value={`$${this.state.CurrentValue}`}
                    />

                    <InfoBox
                        icon={null}
                        color={null}
                        title="Worth"
                        value={`$${this.state.Worth}`}
                    />

                    <InfoBox
                        icon={null}
                        color={null}
                        title="Delta"
                        value={`$ ${this.state.Delta > 0 ? `+${this.state.Delta}` : this.state.Delta}`}
                    />
                </div>
            
            <div className="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-md m-b-15 " style={styles.infoBox}>
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
                          <StackedTile
                              color={styles.forecastRow0.color}
                              titleColor={styles.forecastRow0.titleColor}
                              title="Average Daily Change"
                              value={this.state.ForecastData.DailyChange}
                          />

                        <InfoBox
                            icon={null}
                            color={styles.forecastRow0.color}
                            title="Average Forecast Value"
                            titleColor={styles.forecastRow0.titleColor}
                            value={`$${this.state.ForecastData.ForecastValue}`}
                        />
                    </div>

                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                        <StackedTile
                            color={styles.forecastRow0.color}
                            titleColor={styles.forecastRow0.titleColor}
                            title="Average Daily Trend"
                            value={this.state.ForecastData.Trend}
                        />

                  <InfoBox
                      icon={null}
                      color={styles.forecastRow0.color}
                      titleColor={styles.forecastRow0.titleColor}
                      title="Average Forecast Worth"
                      value={`$${this.state.ForecastData.ForecastWorth}`}
                  />
                </div>

                <div className="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-md m-b-15">
                    <CoinHistory
                        title="Average Forecast"
                        data={this.state.ForecastGraph.Values}
                        dates={this.state.ForecastGraph.Labels}
                        color={grey200} />
                </div>
            </div>

            <div className="row">
                <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                    <StackedTile
                        color={styles.forecastRow1.color}
                        titleColor={styles.forecastRow1.titleColor}
                        title="Eager Daily Change"
                        value={this.state.NearForecastData.DailyChange}
                    />

                    <InfoBox
                        icon={null}
                        color={styles.forecastRow1.color}
                        titleColor={styles.forecastRow1.titleColor}
                        title="Eager Forecast Value"
                        value={`$${this.state.NearForecastData.ForecastValue}`}
                    />
                </div>

                <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                    <StackedTile
                        color={styles.forecastRow1.color}
                        titleColor={styles.forecastRow1.titleColor}
                        title="Eager Daily Trend"
                        value={this.state.NearForecastData.Trend}
                    />

                    <InfoBox
                        icon={null}
                        color={styles.forecastRow1.color}
                        titleColor={styles.forecastRow1.titleColor}
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
                        <StackedTile
                            color={styles.forecastRow2.color}
                            titleColor={styles.forecastRow2.titleColor}
                            title="Skeptical Daily Change"
                            value={this.state.FarForecastData.DailyChange}
                        />

                        <InfoBox
                            icon={null}
                            color={styles.forecastRow2.color}
                            titleColor={styles.forecastRow2.titleColor}
                            title="Skeptical Forecast Value"
                            value={`$${this.state.FarForecastData.ForecastValue}`}
                        />
                    </div>

                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-3 col-md m-b-15">
                        <StackedTile
                            color={styles.forecastRow2.color}
                            titleColor={styles.forecastRow2.titleColor}
                            title="Skeptical Daily Trend"
                            value={this.state.FarForecastData.Trend}
                        />
                                  
                        <InfoBox
                            icon={null}
                            color={styles.forecastRow2.color}
                            titleColor={styles.forecastRow2.titleColor}
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
                                color0={styles.forecastRow0.color} 
                                color1={styles.forecastRow1.color}
                                color2={styles.forecastRow2.color} />
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
