class GenResponse<T>{
    isSuccess: boolean;
    result: T | undefined | null;
    message: string | null;
    error: string | null;
    statCode: StatusCode;

    constructor() {
        this.isSuccess = false;
        this.message = '';
        this.statCode = 200;
        this.error = null;
    }

    static Result<T>(objVal: T, message: string | null = '',statusCode: StatusCode = StatusCode.OK): GenResponse<T> {
        const objResp = new GenResponse<T>();

        objResp.result = objVal;
        if (statusCode === StatusCode.OK || statusCode === StatusCode.Created) {
            objResp.isSuccess = true;
        }
        objResp.message = message;
        objResp.statCode = statusCode;

        return objResp;
    }

    static Failed<T>(objVal: T, error: string, message: string | null = null,statusCode: StatusCode = StatusCode.NotImplemented){
        const objResp = new GenResponse<T>();
        objResp.result = objVal;
        objResp.isSuccess = false;
        objResp.error = error;
        objResp.message = message;
        objResp.statCode = statusCode;
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