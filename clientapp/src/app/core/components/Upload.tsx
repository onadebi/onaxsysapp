import React, { FormEvent } from "react";
import { useRef } from "react";
import { toast } from "react-toastify";
import { useAppStore } from "../../common/services/appservices";
import { useDispatch } from "react-redux";
import { AppDispatch } from "../../../store/storeKeeper";
import { hideLoader, setLoading } from "../../../store/slices/common/Common.slice";

interface IProps {
  children: React.ReactNode;
  fileType: string;
  setProgress: (progress: number) => void;
  setData: (res: string) => void;
}

const Upload: React.FC<IProps> = ({ children, fileType, setProgress, setData }) => {
  const dispatch = useDispatch<AppDispatch>();
  const ref = useRef<HTMLInputElement>(null);
  const { imagesService, commonService } = useAppStore();

  const handleError = (evt: FormEvent<HTMLInputElement>) => {
    evt.preventDefault();
    let uploadError = '';
    evt.currentTarget.onerror = (error) => {
        uploadError = error.toString();
        console.log(error.toString());
    };
    toast.error(`Image upload failed!: ${uploadError}`);
  };
  const handleUpload = async (evt: FormEvent<HTMLInputElement>) => {
    evt.preventDefault();
    dispatch(setLoading({display:true,  message:'Uploading image...'}));
    if (evt.currentTarget.files && evt.currentTarget.files.length > 0) {
        const file = evt.currentTarget.files[0];
        if (!new RegExp(`^${fileType}/.*$`).test(file.type)) {
          toast.error(`Only ${fileType} files are allowed!`);
          dispatch(hideLoader());
          return;
        }
        const resp = await imagesService.uploadImage(evt.currentTarget.files[0]);
        if (resp && resp.message) {
            console.log(resp);
            setData(resp.message);
        }else{
            commonService.LogActivity("Image upload failed. Kindly retry");
        }
    }else{
        commonService.LogActivity("No file selected.");
        dispatch(hideLoader());
        return;
    }
    dispatch(hideLoader());
  };

  const handleUploadProgress = (evt: FormEvent<HTMLInputElement>) => {
    evt.currentTarget.onprogress = (progress) => {
        console.log(progress);
        setProgress(Math.round((progress.loaded / progress.total) * 100));
    };
  };

  return (
    <>
      <div className="fileUpload">
        <input type="file" ref={ref} className="hidden" onError={handleError} onProgress={handleUploadProgress} onChange={handleUpload} />
      </div>
      <div className="cursor-pointer" onClick={() => ref.current?.click()}>
        {children}
      </div>
    </>
  );
};

export default Upload;
