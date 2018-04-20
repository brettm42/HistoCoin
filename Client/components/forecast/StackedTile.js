import React from 'react';
import PropTypes from 'prop-types';
import Avatar from 'material-ui/Avatar';
import InfoBox from '../forecast/InfoBox';
import InlineInfo from '../form/InlineInfo';
import { blue400, grey400, pink400 } from 'material-ui/styles/colors';
import { typography } from 'material-ui/styles';
import ContentRemoveCircle from 'material-ui/svg-icons/content/remove-circle';
import NavigationArrowDropDown from 'material-ui/svg-icons/navigation/arrow-drop-down';
import NavigationArrowDropUp from 'material-ui/svg-icons/navigation/arrow-drop-up';

class StackedTile extends React.Component {

  render() {
    const { color, title, titleColor, value } = this.props;

    const styles = {
        content: { height: 150 },
        status: {
            positive: blue400,
            neutral: grey400,
            negative: pink400
        },
        value: {
            padding: '10px 0 25px 10px'
        }
    };

return (
    <InfoBox
        style={styles.content}
        icon={null}
        color={color}
        title={title}
        titleColor={titleColor}
        value={
            <InlineInfo
                style={styles.value}
                color={color}
                leftValue=
                    {value === 0 || value === null
                        ? <Avatar backgroundColor={styles.status.neutral} icon={<ContentRemoveCircle />} />
                        : (value > 0
                            ? <Avatar backgroundColor={styles.status.positive} icon={<NavigationArrowDropUp />} />
                            : <Avatar backgroundColor={styles.status.negative} icon={<NavigationArrowDropDown />} />)}
                rightValue=
                    {`$ ${value > 0 ? `+${value}` : value}`}
                />}
            />
        );
    }
}

StackedTile.propTypes = {
  color: PropTypes.string,
  title: PropTypes.string,
  titleColor: PropTypes.string,
  value: PropTypes.any
};

export default StackedTile;
