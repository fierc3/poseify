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
  axios.get('/api/GetUserEstimations', config)
    .then((res) => res.data as  IEstimation[]);


function useEstimations() {
  return useQuery(
    claimsKeys.claim,
    async () => fetchData(),
    {
      staleTime: 20000, // 20 seconds
      cacheTime: 60000, // 1 minute
      retry: true
    }
  )
}

export { useEstimations as default }

