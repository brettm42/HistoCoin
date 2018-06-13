import React from 'react';
import PropTypes from 'prop-types';
import { RouteLink } from 'dotnetify/dist/dotnetify-react.router';
import Avatar from 'material-ui/Avatar';
import Drawer from 'material-ui/Drawer';
import Divider from 'material-ui/Divider';
import FontIcon from 'material-ui/FontIcon';
import MenuItem from 'material-ui/MenuItem';
import { spacing, typography } from 'material-ui/styles';
import { white, blue600, grey400 } from 'material-ui/styles/colors';
import style from 'material-ui/svg-icons/image/style';

const Sidebar = (props) => {

  let { vm, logoTitle, open, userAvatarUrl, userBackgroundUrl, menus } = props;

  const styles = {
    logo: {
      cursor: 'pointer',
      fontSize: 22,
      color: typography.textFullWhite,
      lineHeight: `${spacing.desktopKeylineIncrement}px`,
      fontWeight: typography.fontWeightLight,
      backgroundColor: blue600,
      paddingLeft: 70,
      height: 56
    },
    menuItem: {
      color: white,
      fontSize: 14
    },
    divider: {
      margin: 15
    },
    avatar: {
      div: {
        padding: '15px 0 0 15px',
        backgroundImage: 'url(' + userBackgroundUrl + ')',
        height: 45
      },
      detailDiv: {
          padding: '15px 0 25px 15px',
          backgroundImage: 'url(' + userBackgroundUrl + ')',
          height: 45
      },
      icon: {
        float: 'left',
        display: 'block',
        marginRight: 15,
        boxShadow: '0px 0px 0px 8px rgba(0,0,0,0.2)'
      },
      span: {
        paddingTop: 12,
        display: 'block',
        color: 'white',
        fontWeight: 300,
        textShadow: '1px 1px #444'
      },
      emailSpan: {
        paddingTop: 6,
        display: 'block',
        color: 'white',
        fontWeight: 150
      },
      loginSpan: {
          paddingTop: 6,
          display: 'block',
          color: 'white',
          fontWeight: 150
      }
    }
  };

  return (
    <Drawer docked={true} open={open}>
      <div style={styles.logo}>{logoTitle}</div>
      <div style={styles.avatar.div}>
          <Avatar src={userAvatarUrl} size={50} style={styles.avatar.icon} />
          <span style={styles.avatar.span}>{props.username}</span>
      </div>
      <div style={styles.avatar.detailDiv}>
        <span style={styles.avatar.emailSpan}>{props.emailAddress}</span>
        <span style={styles.avatar.loginSpan}>{props.lastLogin}</span>
      </div>
          <Divider style={styles.divider} />
      <div>
        {menus.map((menu, index) =>
          <MenuItem
            key={index}
            style={styles.menuItem}
            primaryText={menu.Title}
            leftIcon={<FontIcon className="material-icons">{menu.Icon}</FontIcon>}
            containerElement={<RouteLink vm={vm} route={menu.Route} />}
          />
        )}
      </div>
    </Drawer>
  );
};

Sidebar.propTypes = {
  sidebarOpen: PropTypes.bool,
  menus: PropTypes.array,
  username: PropTypes.string,
  userAvatarUrl: PropTypes.string,
  userBackgroundUrl: PropTypes.string,
  emailAddress: PropTypes.string,
  lastLogin: PropTypes.string
};

export default Sidebar;
