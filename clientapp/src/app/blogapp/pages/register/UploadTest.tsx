import React, { useState } from "react";
import { useAppStore } from "../../../common/services/appservices";

const UploadTest: React.FC = () => {
  const { imagesService } = useAppStore();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [uploadStatus, setUploadStatus] = useState<string>("");

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      setSelectedFile(event.target.files[0]);
      setUploadStatus(""); // Clear any previous status
    }
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      setUploadStatus("No file selected.");
      return;
    } else {
      const resp = await imagesService.uploadImage(selectedFile);
      if (resp && resp.message) {
        setUploadStatus(resp.message);
      }
    }
  };

  return (
    <>
      <div style={{ maxWidth: "400px", margin: "auto", textAlign: "center" }}>
        <h2>File Upload</h2>
        <input type="file" onChange={handleFileChange} />
        <br />
        <button
          style={{ marginTop: "10px" }}
          onClick={handleUpload}
          disabled={!selectedFile}
        >
          Upload File
        </button>
        {uploadStatus && <p>{uploadStatus}</p>}
      </div>
    </>
  );
};

export default UploadTest;
