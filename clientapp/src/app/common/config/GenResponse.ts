class GenResponse<T>{
    isSuccess: boolean;
    data: T | undefined | null;
    message: string | null;
    error: string | null;
    statusCode: StatusCode;

    constructor() {
        this.isSuccess = false;
        this.message = '';
        this.statusCode = 200;
        this.error = null;
    }

    static Result<T>(objVal: T, message: string | null = '',statusCode: StatusCode = StatusCode.OK): GenResponse<T> {
        const objResp = new GenResponse<T>();

        objResp.data = objVal;
        if (statusCode === StatusCode.OK || statusCode === StatusCode.Created) {
            objResp.isSuccess = true;
        }
        objResp.message = message;
        objResp.statusCode = statusCode;

        return objResp;
    }

    static Failed<T>(objVal: T, error: string, message: string | null = null,statusCode: StatusCode = StatusCode.NotImplemented){
        const objResp = new GenResponse<T>();
        objResp.data = objVal;
        objResp.isSuccess = false;
        objResp.error = error;
        objResp.message = message;
        objResp.statusCode = statusCode;
        return objResp;
    }
}


export enum StatusCode {
    OK = 200,
    Created = 201,
    // NoChanges=304,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    UnAvailableForLegalReasons = 451,
    ServerError = 500,
    NotImplemented = 501,
    ServiceNotAvailable = 503,
    GatewayTimeout = 504,
    InsufficientStorage = 507
}


export default GenResponse;