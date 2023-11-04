import './App.css';
import * as React from "react";
import SimpleTimeTracker from "./views/SimpleTimeTracker"
import ITodays from "./views/ITodays"
import {
  makeStyles,
  shorthands,
  tokens,
  Tab,
  TabList,
} from "@fluentui/react-components";
import {
  AirplaneRegular,
  AirplaneFilled,
  AirplaneTakeOffRegular,
  AirplaneTakeOffFilled,
  TimeAndWeatherRegular,
  TimeAndWeatherFilled,
  bundleIcon,
} from "@fluentui/react-icons";
import type {
  SelectTabData,
  SelectTabEvent,
  TabValue,
} from "@fluentui/react-components";

const Airplane = bundleIcon(AirplaneFilled, AirplaneRegular);
const AirplaneTakeOff = bundleIcon(
  AirplaneTakeOffFilled,
  AirplaneTakeOffRegular
);
const TimeAndWeather = bundleIcon(TimeAndWeatherFilled, TimeAndWeatherRegular);

const useStyles = makeStyles({
  root: {
    alignItems: "flex-start",
    display: "flex",
    flexDirection: "column",
    justifyContent: "flex-start",
    ...shorthands.padding("50px", "20px"),
    rowGap: "20px",
  },
  panels: {
    ...shorthands.padding(0, "10px"),
    "& th": {
      textAlign: "left",
      ...shorthands.padding(0, "30px", 0, 0),
    },
  },
  propsTable: {
    "& td:first-child": {
      fontWeight: tokens.fontWeightSemibold,
    },
    "& td": {
      ...shorthands.padding(0, "30px", 0, 0),
    },
  },
});

function App() {
  const styles = useStyles();

  const [selectedValue, setSelectedValue] =
    React.useState<TabValue>("SimpleTimeTracker");

  const onTabSelect = (event: SelectTabEvent, data: SelectTabData) => {
    setSelectedValue(data.value);
  };

  return (
    <div className={styles.root}>
      <TabList selectedValue={selectedValue} onTabSelect={onTabSelect}>
        <Tab id="SimpleTimeTracker" icon={<Airplane />} value="SimpleTimeTracker">
        SimpleTimeTracker
        </Tab>
        <Tab id="ITodays" icon={<AirplaneTakeOff />} value="ITodays">
        ITodays
        </Tab>
      </TabList>
      <div className={styles.panels}>
        {selectedValue === "SimpleTimeTracker" && <SimpleTimeTracker />}
        {selectedValue === "ITodays" && <ITodays />}
      </div>
    </div>
  );
}

export default App;
