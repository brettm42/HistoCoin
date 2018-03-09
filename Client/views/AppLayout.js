import React from 'react';
import PropTypes from 'prop-types';
import dotnetify from 'dotnetify';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import withWidth, { LARGE, SMALL } from 'material-ui/utils/withWidth';
import Header from '../components/Header';
import Sidebar from '../components/Sidebar';
import ThemeDefault from '../styles/theme-default';
import auth from '../auth';
import { log } from 'util';

class AppLayout extends React.Component {

  constructor(props) {
    super(props);

      this.vm =
        dotnetify.react.connect(
        "AppLayout",
        this,
        {
          headers: { Authorization: "Bearer " + auth.getAccessToken() },
          exceptionHandler: _ => auth.signOut()
        });

    this.vm.onRouteEnter = (path, template) => template.Target = "Content";

    this.state = {
      sidebarOpen: false,
      Menus: []
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  componentWillReceiveProps(nextProps) {
    if (this.props.width !== nextProps.width) {
      this.setState({ sidebarOpen: nextProps.width === LARGE });
    }
  }

  render() {
    let { sidebarOpen, Menus, UserAvatar, UserBackground, UserName, EmailAddress, LastLogin } = this.state;
      let userAvatarUrl = UserAvatar ? UserAvatar : null;
      let userBackgroundUrl = UserBackground ? UserBackground : '../images/material_bg.png';

    const paddingLeftSidebar = 236;
    const styles = {
      header: { paddingLeft: sidebarOpen ? paddingLeftSidebar : 0 },
      container: {
        margin: '80px 20px 20px 15px',
        paddingLeft: sidebarOpen && this.props.width !== SMALL ? paddingLeftSidebar : 0
      }
    };

    const handleSidebarToggle = () => this.setState({ sidebarOpen: !this.state.sidebarOpen });

    const handleSidebarCollapse = () => this.setState({ sidebarOpen: false });

    return (
      <MuiThemeProvider muiTheme={ThemeDefault}>
        <div>
          <Header
              styles={styles.header}
              onSidebarToggle={handleSidebarToggle}
          />
          <Sidebar
            vm={this.vm}
            logoTitle="HistoCoin"
            open={sidebarOpen}
            userAvatarUrl={userAvatarUrl}
            userBackgroundUrl={userBackgroundUrl}
            menus={Menus}
            username={UserName}
            emailAddress={EmailAddress}
            lastLogin={LastLogin}
          />
          <div id="Content" style={styles.container} onFocus={handleSidebarCollapse} onClick={handleSidebarCollapse} />
        </div>
      </MuiThemeProvider>
    );
  }
}

AppLayout.propTypes = {
  userAvatar: PropTypes.string,
  userName: PropTypes.string,
  menus: PropTypes.array,
  width: PropTypes.number
};

export default withWidth()(AppLayout);
