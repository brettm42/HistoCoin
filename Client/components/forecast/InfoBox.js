import React from 'react';
import PropTypes from 'prop-types';
import Paper from 'material-ui/Paper';
import Subheader from 'material-ui/Subheader';
import { white, grey800 } from 'material-ui/styles/colors';
import { typography } from 'material-ui/styles';

class InfoBox extends React.Component {

  render() {
    const { color, title, value, Icon } = this.props;

    const styles = {
      content: {
        height: 120
      },
      number: {
        display: 'block',
        fontWeight: 'bold',
        fontSize: 18,
        padding: '5px 10px',
        color: grey800
      },
      text: {
        fontSize: 16,
        fontWeight: typography.fontWeightLight,
        color: grey800,
        backgroundColor: color
      },
      iconSpan: {
        float: 'left',
        height: 90,
        width: 90,
        textAlign: 'center',
        backgroundColor: color
      },
      icon: {
        height: 48,
        width: 48,
        marginTop: 20,
        maxWidth: '100%'

      }
    };

    return (
        <Paper>
            {Icon ?
                <span style={styles.iconSpan}>
                  <Icon color={white} style={styles.icon} />
                </span>
                : <div />}
            
        <div style={styles.content}>
          <Subheader style={styles.text}>{title ? title : "-"}</Subheader>
          <span style={styles.number}>{value ? value : "-"}</span>
        </div>
      </Paper>
    );
  }
}

InfoBox.propTypes = {
  Icon: PropTypes.any,
  color: PropTypes.string,
  title: PropTypes.string,
  value: PropTypes.any
};

export default InfoBox;
