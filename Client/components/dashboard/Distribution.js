import React from 'react';
import PropTypes from 'prop-types';
import { Doughnut } from 'react-chartjs-2';
import Paper from 'material-ui/Paper';
import Avatar from 'material-ui/Avatar';
import List from 'material-ui/List/List';
import ListItem from 'material-ui/List/ListItem';
import MemoryIcon from 'material-ui/svg-icons/hardware/memory';
import DiskIcon from 'material-ui/svg-icons/hardware/sim-card';
import NetworkIcon from 'material-ui/svg-icons/device/network-wifi';
import { cyan600, pink600, purple600 } from 'material-ui/styles/colors';
import GlobalStyles from '../../styles/styles';

const Distribution = (props) => {

  const styles = {
    paper: {
      minHeight: 344,
      padding: 10
    },
    legend: {
      paddingTop: 10
    },
    legendText: {
      fontSize: '12px'
    },
    pieChartDiv: {
      height: 400,
      textAlign: 'center'
    }
  };

    const labelStyles = [
        { color: cyan600, icon: <MemoryIcon /> },
        { color: pink600, icon: <DiskIcon /> },
        { color: purple600, icon: <NetworkIcon /> },
        { color: cyan600, icon: <MemoryIcon /> },
        { color: pink600, icon: <DiskIcon /> },
        { color: purple600, icon: <NetworkIcon /> },
        { color: cyan600, icon: <MemoryIcon /> },
        { color: pink600, icon: <DiskIcon /> },
        { color: purple600, icon: <NetworkIcon /> },
        { color: cyan600, icon: <MemoryIcon /> },
        { color: pink600, icon: <DiskIcon /> },
        { color: purple600, icon: <NetworkIcon /> },
    ];

  const data = {
    labels: props.label,
    datasets: [{
      data: props.data,
      backgroundColor: [cyan600, pink600, purple600, cyan600, pink600, purple600, cyan600, pink600, purple600, cyan600, pink600, purple600]
    }]
  };

  const options = {
    legend: { display: false },
    layout: { padding: { left: 0, right: 10, top: 20, bottom: 10 } },
    maintainAspectRatio: false,
    multiTooltipTemplate: "<%= datasetLabel %> - <%= value %>"
  };

  return (
    <Paper style={styles.paper}>
        <span style={GlobalStyles.title}>Distribution</span>
      <div className="row">
        <div className="col-xs-12 col-sm-8 col-md-8 col-lg-8">
          <div style={styles.pieChartDiv}>
            <Doughnut data={data} options={options} />
          </div>
        </div>
        <div className="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div style={styles.legend}>
              <List>
                {props.label.map((item, idx) =>
                  <ListItem
                    key={item}
                    leftAvatar={<Avatar icon={labelStyles[idx].icon} backgroundColor={labelStyles[idx].color} />}>
                    <span style={styles.legendText}>{item + " " + props.data[idx] + "%"}</span>
                  </ListItem>
                )}
              </List>
            </div>
        </div>
      </div>
    </Paper>
  );
};

Distribution.propTypes = {
  data: PropTypes.array
};

export default Distribution;
