import React from 'react';
import dotnetify from 'dotnetify';
import { RouteLink } from 'dotnetify/dist/dotnetify-react.router';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import FloatingActionButton from 'material-ui/FloatingActionButton';
import Snackbar from 'material-ui/Snackbar';
import TextField from 'material-ui/TextField';
import { Table, TableBody, TableHeader, TableHeaderColumn, TableRow, TableRowColumn } from 'material-ui/Table';
import IconRemove from 'material-ui/svg-icons/content/clear';
import ContentAdd from 'material-ui/svg-icons/content/add';
import { pink500, grey200, grey500 } from 'material-ui/styles/colors';
import BasePage from '../components/BasePage';
import Pagination from '../components/table/Pagination';
import InlineEdit from '../components/table/InlineEdit';
import ThemeDefault from '../styles/theme-default';

class TablePage extends React.Component {

  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect("Table", this);
    this.dispatch = state => this.vm.$dispatch(state);

    this.state = {
      addName: '',
      Coins: [],
      Pages: [],
      ShowNotification: false
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    let { addName, Coins, Pages, SelectedPage, ShowNotification } = this.state;

    const styles = {
      addButton: { margin: '1em' },
      removeIcon: { fill: grey500 },
      columns: {
        id: { width: '10%' },
        handle: { width: '25%' },
        count: { width: '35%' },
        startingValue: { width: '35%' },
        lastUpdate: {width: '40%'},
        remove: { width: '20%' }
      },
      pagination: { marginTop: '1em' }
    };

    const handleAdd = _ => {
      if (addName) {
        this.dispatch({ Add: addName + " 0 0" });
        this.setState({ addName: '' });
      }
    };

    const handleUpdate = coin => {
      let newState = Coins.map(item => item.Id === coin.Id ? Object.assign(item, coin) : item);
      this.setState({ Coins: newState });
      this.dispatch({ Update: coin });
    };

    const handleSelectPage = page => {
      const newState = { SelectedPage: page };
      this.setState(newState);
      this.dispatch(newState);
    };

    const hideNotification = _ => this.setState({ ShowNotification: false });

    return (
      <MuiThemeProvider muiTheme={ThemeDefault}>
        <BasePage title="Coin List" navigation="HistoCoin / Coin List">
          <div>
            <div>
              <FloatingActionButton onClick={handleAdd}
                style={styles.addButton}
                backgroundColor={pink500}
                mini={true}
              >
                <ContentAdd />
              </FloatingActionButton>
              <TextField id="AddName" floatingLabelText="Add" hintText="Type coin handle here"
                floatingLabelFixed={true}
                value={addName}
                onKeyPress={event => event.key === "Enter" ? handleAdd() : null}
                onChange={event => this.setState({ addName: event.target.value })} />
            </div>

            <Table>
              <TableHeader>
                <TableRow>
                  <TableHeaderColumn style={styles.columns.handle}>Handle</TableHeaderColumn>
                  <TableHeaderColumn style={styles.columns.count}>Count</TableHeaderColumn>
                  <TableHeaderColumn style={styles.columns.startingValue}>Starting Value</TableHeaderColumn>
                  <TableHeaderColumn style={styles.columns.lastUpdate}>Last Updated</TableHeaderColumn>
                  <TableHeaderColumn style={styles.columns.remove}>Remove</TableHeaderColumn>
                </TableRow>
              </TableHeader>
              <TableBody>
                {Coins.map(item =>
                  <TableRow key={item.Id}>
                    <TableRowColumn style={styles.columns.handle}>
                      <InlineEdit onChange={value => handleUpdate({ Id: item.Id, Handle: value })}>{item.Handle}</InlineEdit>
                    </TableRowColumn>
                    <TableRowColumn style={styles.columns.count}>
                      <InlineEdit onChange={value => handleUpdate({ Id: item.Id, Count: value })}>{item.Count}</InlineEdit>
                    </TableRowColumn>
                    <TableRowColumn style={styles.columns.startingValue}>
                      <InlineEdit onChange={value => handleUpdate({ Id: item.Id, StartingValue: value })}>{item.StartingValue}</InlineEdit>
                    </TableRowColumn>
                    <TableRowColumn style={styles.columns.lastUpdate}>{item.LastUpdate}</TableRowColumn>
                    <TableRowColumn style={styles.columns.remove}>
                      <FloatingActionButton onClick={_ => this.dispatch({ Remove: item.Id })}
                        zDepth={0}
                        mini={true}
                        backgroundColor={grey200}
                        iconStyle={styles.removeIcon}
                      >
                        <IconRemove />
                      </FloatingActionButton>
                    </TableRowColumn>
                  </TableRow>
                )}
              </TableBody>
            </Table>

            <Pagination style={styles.pagination}
              pages={Pages}
              select={SelectedPage}
              onSelect={handleSelectPage}
            />

            <Snackbar open={ShowNotification} message="Changes saved" autoHideDuration={1000} onRequestClose={hideNotification} />
          </div>
        </BasePage>
      </MuiThemeProvider>
    );
  }
}

export default TablePage;
