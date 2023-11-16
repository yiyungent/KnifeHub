import React, { useState, useEffect, useRef } from "react";
import * as echarts from "echarts";
import { apiUpload } from "../../api/SimpleTimeTracker";

let echartObj: any = null;

const SimpleTimeTracker = () => {
  //#region file upload
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState<
    "initial" | "uploading" | "success" | "fail"
  >("initial");

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setStatus("initial");
      setFile(e.target.files[0]);
    }
  };

  const handleUpload = async () => {
    if (file) {
      console.log("echartObj", echartObj);
      echartObj.showLoading();
      console.log("Uploading file...");

      const formData = new FormData();
      formData.append("file", file);

      try {
        const res: any = await apiUpload(file);

        console.log(res);
        setStatus("success");

        // res.data.title.text = "";
        // res.data.toolbox.feature = {};
        echartObj.setOption(res.data);
        echartObj.hideLoading();
      } catch (error) {
        console.error(error);
        setStatus("fail");
      }
    }
  };
  //#endregion

  const echartRef = useRef(null);
  useEffect(() => {
    console.log("init");
    console.log("echartRef", echartRef);
    echartObj = echarts.init(echartRef.current);
    console.log("echartObj", echartObj);
    echartObj.resize();
  }, []);

  return (
    <>
      <div style={{ width: "100%" }}>
        <div className="input-group">
          <label htmlFor="file" className="sr-only">
            Choose a file
          </label>
          <input id="file" type="file" onChange={handleFileChange} />
        </div>
        {file && (
          <section>
            File details:
            <ul>
              <li>Name: {file.name}</li>
              <li>Type: {file.type}</li>
              <li>Size: {file.size} bytes</li>
            </ul>
          </section>
        )}

        {file && (
          <button onClick={handleUpload} className="submit">
            Upload a file
          </button>
        )}

        <Result status={status} />
      </div>
      <div ref={echartRef} style={{ width: "100%", height: "800px" }}></div>
    </>
  );
};

const Result = ({ status }: { status: string }) => {
  if (status === "success") {
    return <p>✅ File uploaded successfully!</p>;
  } else if (status === "fail") {
    return <p>❌ File upload failed!</p>;
  } else if (status === "uploading") {
    return <p>⏳ Uploading selected file...</p>;
  } else {
    return null;
  }
};

export default SimpleTimeTracker;
