import React from 'react';
import PropTypes from 'prop-types';
import { Bar } from 'react-chartjs-2';
import Paper from 'material-ui/Paper';
import { white, pink600, pink500, pink400, pink200 } from 'material-ui/styles/colors';
import GlobalStyles from '../../styles/styles';

const Deltas = (props) => {

  const styles = {
    paper: {
      backgroundColor: pink600,
      height: 150
    },
    div: {
      marginLeft: 'auto',
      marginRight: 'auto',
      width: '95%',
      height: 85
    },
    header: {
      color: white,
      backgroundColor: pink500,
      padding: 10
    }
  };

  const data = {
    labels: props.label,
    datasets: [
      {
        data: props.data ? props.data : Array(props.label.length).fill(0.5),
        backgroundColor: pink200,
        borderColor: pink500
      }
    ]
  };

    const options = {
        legend: { display: false },
        scales: {
            xAxes: [{ ticks: { fontColor: white }, display: true, gridLines: { display: false } }],
            yAxes: [{ display: false, min: 0, suggestedMax: 10 }]
        },
        layout: { padding: { bottom: 5 } },
        maintainAspectRatio: false,
        multiTooltipTemplate: "<%= datasetLabel %> - <%= value %>"
    };

  return (
    <Paper style={styles.paper}>
      <div style={{ ...GlobalStyles.title, ...styles.header }}>Current Delta (USD)</div>
      <div style={styles.div}>
        <Bar data={data} options={options} />
      </div>
    </Paper>
  );
};

Deltas.propTypes = {
  data: PropTypes.array
};

export default Deltas;
