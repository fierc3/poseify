import {env} from '../config';
import { Methods, IApiData, IProblemDetails } from './api.types';

const isParamEmpty = (param:  Record<string, string>) => {
    const value= param[Object.keys(param)[0]];
    return value === undefined || value.length < 1
}

const evaluateSearchParams = (params: Record<string, string>[]): string => {
    if(params.length === 0 || params.filter(x => !isParamEmpty(x)).length === 0) return ''

    return '?' + (params.map((x) => {
        const keys = Object.keys(x)
       return keys[0] + '=' + x[keys[0]]}).join('&')
    );
}

const fetchWithDefaults = async (endpointName: string, method: string, params:Record<string, string>[]): Promise<IApiData | IProblemDetails> =>  {
	const unparsed = await fetch(env.backend.baseUrl + '/' + endpointName + evaluateSearchParams(params), {
		method: method,
	})

    if(unparsed.ok){
        return await unparsed.json() as IApiData;
    }

    return unparsed.json() as IProblemDetails;
}

export const isReturnData = (data: any) => {
    return data['traceId'] === undefined
}