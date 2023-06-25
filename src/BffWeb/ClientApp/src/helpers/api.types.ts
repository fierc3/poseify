export interface IUserProfile {
    displayName: string,
    token: string,
    imageurl: string
}

export interface IApiData {
    // extend when new endpoint is defined 
    data: string | IUserProfile;
}

export enum Methods {
    Get = "GET"
}

export interface IProblemDetails {
    type?: string | undefined;
    title?: string | undefined;
    status?: number | undefined;
    detail?: string | undefined;
    instance?: string | undefined;
    extensions?: { [key: string]: Object; } | undefined;
    [key: string]: any;
}

export enum EstimationState{
    Success = "Success",
    Processing ="Processing",
    Failed = "Failed",
    Queued ="Queued",
}

export interface IEstimation {
    internalGuid: string;
    displayName: string;
    tags: string[];
    uploadingProfile: string;
    modifiedDate: Date;
    state: EstimationState,
    stateText: string
}

export enum AttachmentType{
    Joints = "Joints",
    Preview ="Preview",
    Npz = "Npz",
    Bvh = "Bvh"
}

