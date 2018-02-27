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
import ValueHistory from '../components/form/ValueHistory';
import { grey400, pink400, orange200 } from 'material-ui/styles/colors';
import BasePage from '../components/BasePage';
import ThemeDefault from '../styles/theme-default';

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
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    let { dirty, Coins, Id, Handle, Count, StartingValue, Worth, CurrentValue, Delta, HistoricalDates, HistoricalValues } = this.state;

    const styles = {
      selectLabel: { color: pink400 },
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
      saveButton: { marginLeft: 5 }
    };

    const handleSelectFieldChange = (event, idx, value) => this.routeTo(Coins.find(i => i.Id == value).Route);

    const handleCancel = _ => {
      this.dispatch({ Cancel: Id });
      this.setState({ dirty: false });
    }

    const handleSave = _ => {
      this.dispatch({ Save: { Id: Id, Handle: Handle, Count: Count, StartingValue: StartingValue } });
      this.setState({ dirty: false });
    }

    return (
      <MuiThemeProvider muiTheme={ThemeDefault}>
        <BasePage title="Form Page" navigation="HistoCoin / Form Page">
            <div>
          <form>
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
                      <ValueHistory data={this.state.HistoricalValues} dates={this.state.HistoricalDates} />
                    </div>
                </div>
            </div>
        </BasePage>
      </MuiThemeProvider>
    );
  }
}

export default FormPage;
