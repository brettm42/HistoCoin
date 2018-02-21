import React from 'react';
import dotnetify from 'dotnetify';
import { RouteLink } from 'dotnetify/dist/dotnetify-react.router';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import Divider from 'material-ui/Divider';
import MenuItem from 'material-ui/MenuItem';
import RaisedButton from 'material-ui/RaisedButton';
import SelectField from 'material-ui/SelectField';
import TextField from 'material-ui/TextField';
import { grey400, pink400 } from 'material-ui/styles/colors';
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
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    let { dirty, Coins, Id, Handle, Count, StartingValue } = this.state;

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
        </BasePage>
      </MuiThemeProvider>
    );
  }
}

export default FormPage;
