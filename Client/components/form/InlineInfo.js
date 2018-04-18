import React from 'react';
import PropTypes from 'prop-types';
import Paper from 'material-ui/Paper';
import { white, grey800 } from 'material-ui/styles/colors';
import { typography } from 'material-ui/styles';

class InlineInfo extends React.Component {

  render() {
    const { color, leftValue, rightValue } = this.props;

    const styles = {
      content: {
        padding: '2px 2px',
        height: 80
      },
      number: {
        display: 'block',
        fontWeight: 'bold',
        fontSize: 18,
        paddingTop: 10,
        color: color ? white : grey800
      },
      text: {
        fontSize: 18,
        fontWeight: typography.fontWeightLight,
        color: color ? white : grey800,
        padding: '5px 10px'
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
        <div style={styles.content}>
          <span style={styles.text}>{leftValue}</span>
          <span style={styles.text}>{rightValue}</span>
        </div>
    );
  }
}

InlineInfo.propTypes = {
  color: PropTypes.string,
  leftValue: PropTypes.any,
  rightValue: PropTypes.any
};

export default InlineInfo;
