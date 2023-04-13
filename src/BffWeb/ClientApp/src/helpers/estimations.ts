import axios from 'axios';
import { useQuery } from 'react-query';
import { IEstimation } from './api.types';

const claimsKeys = {
  claim: ['estimations']
}

const config = {
  headers: {
    'X-CSRF': '1'
  }
}

const fetchData = async () =>
  axios.get('/api/GetUserEstimations?userGuid=1', config)
    .then((res) => res.data as  IEstimation[]);


function useEstimations() {
  return useQuery(
    claimsKeys.claim,
    async () => fetchData(),
    {
      staleTime: Infinity,
      cacheTime: Infinity,
      retry: false
    }
  )
}

export { useEstimations as default }

