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

class FormPage extends React.Component {

  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect("Form", this);
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
      Trend: '',
      Ath: ''
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    let { dirty, Coins, Id, Handle, Count, StartingValue } = this.state;

    const styles = {
        selectLabel: { color: pink400 },
        form: {
            paddingBottom: 50
        },
        divider: {
            marginBottom: 45
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
        infoDiv: {
            padding: '25px 25px'
        },
      trendDiv: {
        height: '300px'
      },
      saveButton: { marginLeft: 5 }
    };

    const handleSelectFieldChange = (event, idx, value) => this.routeTo(Coins.find(i => i.Id === value).Route);

      const handleCancel = _ => {
          this.dispatch({ Cancel: Id });
          this.setState({ dirty: false });
      };

      const handleSave = _ => {
          this.dispatch({ Save: { Id: Id, Handle: Handle, Count: Count, StartingValue: StartingValue } });
          this.setState({ dirty: false });
      };

    return (
        <MuiThemeProvider muiTheme={ThemeDefault}>
            <div>
        <BasePage title="Coin Details" navigation="HistoCoin / Coin Details">
            <div>
          <form style={styles.form}>
            <SelectField
              value={Id}
              onChange={handleSelectFieldChange}
              floatingLabelText="Select to edit"
              floatingLabelStyle={styles.selectLabel}
            >
              {Coins.map(item =>
                <MenuItem key={item.Id} value={item.Id} primaryText={`${item.Handle} (${item.Count})`} />
              )}
            </SelectField>

            <TextField
              hintText="Enter cryptocurrency handle"
              floatingLabelText="Coin Handle"
              fullWidth={true}
              value={Handle}
              onChange={event => this.setState({ Handle: event.target.value, dirty: true })} />

            <TextField
              hintText="Enter current wallet holdings"
              floatingLabelText="Number of coins in wallet"
              fullWidth={true}
              value={Count}
              onChange={event => this.setState({ Count: event.target.value, dirty: true })} />

            <TextField
                hintText="Enter cost in USB per coin on purchase"
                floatingLabelText="Cost per coin when purchased"
                fullWidth={true}
                value={StartingValue}
                onChange={event => this.setState({ StartingValue: event.target.value, dirty: true })} />

            <div style={styles.buttons}>
              <RaisedButton label="Cancel"
                onClick={handleCancel}
                disabled={!dirty} />

              <RaisedButton label="Save"
                onClick={handleSave}
                disabled={!dirty}
                style={styles.saveButton}
                primary={true} />
              </div>
            </form>
          </div>
        </BasePage>

        <div className="row" style={styles.infoDiv}>
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
                    title="Historical High (USD)"
                    value={`$${this.state.Ath}`}
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

              <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 col-md m-b-15">
                  <InfoBar
                    icon={null}
                    color={orange200}
                    title="Overall Trend"
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

            <div className="row" style={styles.trendDiv}>
              <div className="col-xs-12 col-sm-12 col-md-12 col-lg-12 col-md m-b-15">
                <CoinHistory
                    data={this.state.HistoricalValues}
                    dates={this.state.HistoricalDates}
                    color={grey200} />
              </div>
            </div>
        </div>
    </MuiThemeProvider>
    );
  }
}

export default FormPage;
