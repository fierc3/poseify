import axios from 'axios';
import { useQuery } from 'react-query';
import { IEstimation } from './api.types';

const idKeys = {
  idk: ['props']
}

const config = {
  headers: {
    'X-CSRF': '1'
  }
}

const fetchData = async () =>
  axios.get('/bff/Identity/GetIdToken', config)
    .then((res) => res.data);


function useAuthentication() {
  return useQuery(
    idKeys.idk,
    async () => fetchData(),
    {
      staleTime: Infinity,
      cacheTime: Infinity,
      retry: false
    }
  )
}

export { useAuthentication as default }

