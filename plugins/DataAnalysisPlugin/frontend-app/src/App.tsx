import "./App.css";
import * as React from "react";
import SimpleTimeTracker from "./views/SimpleTimeTracker";
import ITodays from "./views/ITodays";
import AccountingDuck from "./views/AccountingDuck";
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
    <div className={styles.root} style={{ height: "100%" }}>
      <TabList
        selectedValue={selectedValue}
        onTabSelect={onTabSelect}
        style={{ width: "100%" }}
      >
        <Tab
          id="SimpleTimeTracker"
          icon={<Airplane />}
          value="SimpleTimeTracker"
        >
          SimpleTimeTracker
        </Tab>
        <Tab id="ITodays" icon={<AirplaneTakeOff />} value="ITodays">
          爱今天/时间朋友
        </Tab>
        <Tab id="AccountingDuck" icon={<AirplaneTakeOff />} value="AccountingDuck">
          记账鸭
        </Tab>
      </TabList>
      <div className={styles.panels} style={{ width: "100%", height: "100%" }}>
        {selectedValue === "SimpleTimeTracker" && <SimpleTimeTracker />}
        {selectedValue === "ITodays" && <ITodays />}
        {selectedValue === "AccountingDuck" && <AccountingDuck />}
      </div>
    </div>
  );
}

export default App;
