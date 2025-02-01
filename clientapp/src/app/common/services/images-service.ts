import agent from "../api/agent";
import apiRoutes from "../api/apiRoutes";
import appsettings from "../config/appsettings";
import GenResponse, { StatusCode } from "../config/GenResponse";

export default class ImagesService {

    uploadImage = async (image: File): Promise<GenResponse<boolean>> => {
        let objResp = GenResponse.Failed(false, 'Image upload failed', 'Image upload failed', StatusCode.BadRequest);
        if (image) {
            const allowedTypes = appsettings.FileUploadedConstraints.images;
            if (allowedTypes.includes(image.type)) {
                const formData = new FormData();
                formData.append('image', image);
                try {
                    const resp = await agent.axios.post<GenResponse<boolean>>(apiRoutes.blog.uploadImage, formData);
                    objResp = resp.data;
                } catch(err) {
                    console.log(err);
                    objResp.error = 'An errorr occurred while uploading image. kindly retry.';
                    objResp.result = false;
                }
            } else {
                objResp.message = 'Invalid file uploaded.';
                objResp.error = `Invalid file/fileformat uploaded. Valid formats are: [${appsettings.FileUploadedConstraints.images.join(', ')}].`;
            }
        } else {
            objResp.message = objResp.error = 'No image uploaded';
        }
        return objResp;
    }

}