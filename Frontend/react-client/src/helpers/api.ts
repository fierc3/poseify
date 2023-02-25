import {env} from '../config';
import { IDefaultReturn, ErrorCodes, Methods } from './api.types';

const fetchWithDefaults = async (endpointName: string, method: string, headers:HeadersInit) =>  {
	const unparsed = await fetch(env.backend.baseUrl + '/' + endpointName, {
		method: method,
		headers: headers,
	})

    if(unparsed.ok){
        return await unparsed.json() as IDefaultReturn;
    }

    return {error: {errorCode: ErrorCodes.Unhandled, errorText: 'No server response'}, success: false, data: undefined}
}


export const getCurrentUserProfile = () => {
    return fetchWithDefaults('UserProfile', Methods.Get, []);
}