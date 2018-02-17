import React from 'react';
import dotnetify from 'dotnetify';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import ActionCached from 'material-ui/svg-icons/action/cached';
import ActionTimeline from 'material-ui/svg-icons/action/timeline';
import ActionAccountBalanceWalletIcon from 'material-ui/svg-icons/action/account-balance-wallet';
import { cyan600, pink600, purple600, orange600 } from 'material-ui/styles/colors';
import InfoBox from '../components/dashboard/InfoBox';
import ValueHistory from '../components/dashboard/ValueHistory';
import Deltas from '../components/dashboard/Deltas';
import Distribution from '../components/dashboard/Distribution';
import RecentActivities from '../components/dashboard/RecentActivities';
import FloatingActionButton from 'material-ui/FloatingActionButton';
import globalStyles from '../styles/styles';
import ThemeDefault from '../styles/theme-default';
import auth from '../auth';

class Dashboard extends React.Component {

  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect("Dashboard", this, {
      exceptionHandler: ex => {
         alert(ex.message);
         auth.signOut();
      }
    });

    this.dispatch = state => this.vm.$dispatch(state);

    this.state = {
        Currencies: [],
        Value: [],
        TotalValueUsd: [],
        TotalValueBtc: [],
        CurrentDeltas: [],
        OverallDelta: [],
      DistributionUsd: [],
      DistributionBtc: [],
      CurrentValues: [],
      IsSyncing: false
    };
  }
    
  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
      const styles = {
          syncButton: {
              marginTop: "-50",
              float: "right"
          }
      };

      const handleSync = _ => {
          this.dispatch({ Sync: '' });
      };

    return (
      <MuiThemeProvider muiTheme={ThemeDefault}>
        <div>
          <h3 style={globalStyles.navigation}>HistoCoin / Dashboard</h3>

            <div>
                <FloatingActionButton
                    style={styles.syncButton}
                    disabled={this.state.IsSyncing}
                    onClick={() => handleSync()}
                    mini={true}
                    backgroundColor={pink600}>
                    <ActionCached />
                </FloatingActionButton>
            </div>

          <div className="row">
            <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 m-b-15 ">
                <InfoBox Icon={ActionAccountBalanceWalletIcon}
                    color={orange600}
                    title="Value (USD)"
                    value={`$${this.state.TotalValueUsd}`}
              />
            </div>
                    
              <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 m-b-15 ">
                  <InfoBox Icon={ActionAccountBalanceWalletIcon}
                    color={pink600}
                    title="Value (BTC)"
                    value={`${this.state.TotalValueBtc} BTC`}
                  />
                    </div>

              <div className="col-xs-12 col-sm-6 col-md-3 col-lg-3 m-b-15 ">
                  <InfoBox Icon={ActionTimeline}
                    color={purple600}
                    title="Overall Delta"
                    value={`$ ${this.state.OverallDelta > 0 ? `+${this.state.OverallDelta}` : this.state.OverallDelta}`}
                  />
              </div>
          </div>

          <div className="row">
            <div className="col-xs-12 col-sm-6 col-md-6 col-lg-6 col-md m-b-15">
                <ValueHistory data={this.state.Value} />
              </div>

            <div className="col-xs-12 col-sm-6 col-md-6 col-lg-6 m-b-15">
                <Deltas data={this.state.CurrentDeltas} label={this.state.Currencies} />
            </div>
          </div>

          <div className="row">
            <div className="col-xs-12 col-sm-12 col-md-6 col-lg-6 m-b-15 ">
              <RecentActivities vm={this.vm} data={this.state.CurrentValues} />
            </div>
                    
            <div className="col-xs-12 col-sm-12 col-md-6 col-lg-6 m-b-15 ">
                <Distribution data={this.state.DistributionBtc} label={this.state.Currencies} />
            </div>

          </div>
        </div>
      </MuiThemeProvider>
    );
  }
}

export default Dashboard;
