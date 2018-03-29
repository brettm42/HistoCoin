import React from 'react';
import dotnetify from 'dotnetify';
import { RouteLink } from 'dotnetify/dist/dotnetify-react.router';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import Divider from 'material-ui/Divider';
import MenuItem from 'material-ui/MenuItem';
import RaisedButton from 'material-ui/RaisedButton';
import SelectField from 'material-ui/SelectField';
import TextField from 'material-ui/TextField';
import InfoBar from '../components/form/InfoBar';
import CoinHistory from '../components/form/CoinHistory';
import { grey200, grey400, pink400, orange200 } from 'material-ui/styles/colors';
import BasePage from '../components/BasePage';
import InlineInfo from '../components/form/InlineInfo';
import ThemeDefault from '../styles/theme-default';
import Avatar from 'material-ui/Avatar';
import ContentRemoveCircle from 'material-ui/svg-icons/content/remove-circle';
import NavigationArrowDropDown from 'material-ui/svg-icons/navigation/arrow-drop-down';
import NavigationArrowDropUp from 'material-ui/svg-icons/navigation/arrow-drop-up';

class ForecastPage extends React.Component {

  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect("Forecast", this);
    this.dispatch = state => this.vm.$dispatch(state);
    this.routeTo = route => this.vm.$routeTo(route);

    this.state = {
      dirty: false,
      Coins: [],
      Handle: '',
      Count: '',
      StartingValue: '',
      Worth: '',
      CurrentValue: '',
      Delta: '',
      HistoricalValues: [],
      HistoricalDates: [],
      DailyChange: '',
      Trend: ''
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    let { dirty, Coins, Id, Handle, Count, StartingValue, Worth, CurrentValue, Delta, HistoricalDates, HistoricalValues, DailyChange, Trend } = this.state;

    const styles = {
        selectLabel: { color: pink400 },
        form: {
            paddingBottom: 80
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
      saveButton: { marginLeft: 5 }
    };

    const handleSelectFieldChange = (event, idx, value) => this.routeTo(Coins.find(i => i.Id === value).Route);
      
    return (
      <MuiThemeProvider muiTheme={ThemeDefault}>
        <BasePage title="Forecasting" navigation="HistoCoin / Forecasting">
            <div>
          <form style={styles.form}>
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

            <TextField
              hintText="Coin handle"
              floatingLabelText="Coin Handle"
              fullWidth={true}
              value={Handle} />

            <TextField
              hintText="Current wallet holdings"
              floatingLabelText="Number of coins in wallet"
              fullWidth={true}
              value={Count} />

            <TextField
                hintText="Cost in USB per coin on purchase"
                floatingLabelText="Cost per coin when purchased"
                fullWidth={true}
                value={StartingValue} />
            </form>
                    
          <div className="row">
              <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 m-b-15 ">
                  <InfoBar
                      icon={null}
                      color={orange200}
                      title="Current Value (USD)"
                      value={`$${this.state.CurrentValue}`}
                  />
              </div>
                <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 m-b-15 ">
                    <InfoBar
                        icon={null}
                        color={orange200}
                        title="Worth (USD)"
                        value={`$${this.state.Worth}`}
                    />
                </div>

                <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 m-b-15 ">
                    <InfoBar
                        icon={null}
                        color={orange200}
                        title="Delta"
                        value={`$ ${this.state.Delta > 0 ? `+${this.state.Delta}` : this.state.Delta}`}
                    />
                </div>
            </div>

            <div className="row">
                <div className="col-xs-12 col-sm-6 col-md-6 col-lg-6 col-md m-b-15">
                <CoinHistory
                    data={this.state.HistoricalValues}
                    dates={this.state.HistoricalDates}
                    color={grey200} />
                    </div>

            <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 col-md m-b-15">
                <InfoBar style={styles.trendDiv}
                    icon={null}
                    color={orange200}
                    title="Daily Change (USD)"
                    value={<InlineInfo 
                        leftValue=
                        {this.state.DailyChange === 0 || this.state.DailyChange === null
                            ? <Avatar icon={<ContentRemoveCircle />} />
                            : (this.state.DailyChange > 0
                                ? <Avatar icon={<NavigationArrowDropUp />} />
                                : <Avatar icon={<NavigationArrowDropDown />} />)}
                        rightValue={`$ ${this.state.DailyChange > 0 ? `+${this.state.DailyChange}` : this.state.DailyChange}`} />}
                />
            </div>

            <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 col-md m-b-15">
                <InfoBar style={styles.trendDiv}
                    icon={null}
                    color={orange200}
                    title="Daily Trend (USD)"
                    value={<InlineInfo
                        leftValue=
                        {this.state.Trend === 0 || this.state.Trend === null
                            ? <Avatar icon={<ContentRemoveCircle />} />
                            : (this.state.Trend > 0
                                ? <Avatar icon={<NavigationArrowDropUp />} />
                                : <Avatar icon={<NavigationArrowDropDown />} />)}
                        rightValue={`${this.state.Trend > 0 ? `+${this.state.Trend}` : this.state.Trend} %`} />}
                />
            </div>
          </div>
        </div>
      </BasePage>
    </MuiThemeProvider>
    );
  }
}

export default ForecastPage;
