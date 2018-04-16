import React from 'react';
import PropTypes from 'prop-types';
import { Line } from 'react-chartjs-2';
import Paper from 'material-ui/Paper';
import { white, purple600, purple500 } from 'material-ui/styles/colors';

const ForecastHistory = (props) => {

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
    labels: props.labelsSet ? props.labelsSet : new Array(0),
    datasets: [
      {
            data: props.dataSet0 ? props.dataSet0 : new Array(0),
            fill: false,
            backgroundColor: props.color0,
            borderColor: '#8884d8',
            borderWidth: 2,
            pointBorderWidth: 2,
            cubicInterpolationMode: 'monotone'
        },
        {
            data: props.dataSet1 ? props.dataSet1 : new Array(0),
            fill: false,
            backgroundColor: props.color1,
            borderColor: '#8884d8',
            borderWidth: 2,
            pointBorderWidth: 2,
            cubicInterpolationMode: 'monotone'
        },
        {
            data: props.dataSet2 ? props.dataSet2 : new Array(0),
            fill: false,
            backgroundColor: props.color2,
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
      <div style={{ ...styles.header }}>Forecast Trends</div>
      <div style={styles.div}>
        <Line data={data} options={options} />
      </div>
    </Paper>
  );
};

ForecastHistory.propTypes = {
    dataSet0: PropTypes.array,
    dataSet1: PropTypes.array,
    dataSet2: PropTypes.array,
    labelsSet: PropTypes.array,
    color0: PropTypes.string,
    color1: PropTypes.string,
    color2: PropTypes.string
};

export default ForecastHistory;
