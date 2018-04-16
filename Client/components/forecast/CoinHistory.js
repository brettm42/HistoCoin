import React from 'react';
import PropTypes from 'prop-types';
import { Line } from 'react-chartjs-2';
import Paper from 'material-ui/Paper';
import { white, purple600, purple500 } from 'material-ui/styles/colors';

const CoinHistory = (props) => {

  const styles = {
    paper: {
        backgroundColor: 'white',
      height: 220
    },
    div: {
      padding: '5px 15px 0 15px'
    },
    header: {
      fontSize: 24,
      color: 'black',
      backgroundColor: 'white',
      padding: 10
    }
  };

  const data = {
    labels: props.dates ? props.dates : new Array(0),
    datasets: [
      {
        data: props.data ? props.data : new Array(0),
        fill: false,
        backgroundColor: props.color,
        borderColor: '#8884d8',
        borderWidth: 2,
        pointBorderWidth: 2,
        cubicInterpolationMode: 'monotone'
      }
    ]
  };

  const options = {
    legend: { display: false },
    scales: { xAxes: [{ display: false }], yAxes: [{ display: true }] },
    layout: { padding: { left: 5, right: 5, top: 5, bottom: 5 } },
    maintainAspectRatio: false,
    multiTooltipTemplate: "<%= datasetLabel %> - <%= value %>"
  }

  return (
    <Paper style={styles.paper}>
      <div style={{ ...styles.header }}>Value History</div>
      <div style={styles.div}>
        <Line data={data} options={options} />
      </div>
    </Paper>
  );
};

CoinHistory.propTypes = {
    data: PropTypes.array,
    dates: PropTypes.array,
    color: PropTypes.string
};

export default CoinHistory;
