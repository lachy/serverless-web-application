import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import { useAuth0 } from '@auth0/auth0-react';
import { useDispatch } from 'react-redux';
import { displayError } from '../actions/shared';

const apiUrl = process.env.REACT_APP_SERVER_URL as string;
const api = axios.create({
  baseURL: apiUrl,
});

const useAxios = () => {
  const { getAccessTokenSilently } = useAuth0();
  const dispatch = useDispatch();

  api.interceptors.request.use(
    async (config: AxiosRequestConfig) => {
      const token = await getAccessTokenSilently();
      return {
        ...config,
        headers: { ...config.headers, Authorization: `Bearer ${token}` },
      };
    },
    (error) => error,
  );

  api.interceptors.response.use((response) => response, (error) => {
    dispatch(displayError(error));
    throw error;
  });

  return {
    get: async (url: string, config?: AxiosRequestConfig<any> | undefined)
    : Promise<AxiosResponse> => axios.get(url, config),
    del: async (url: string, config?: AxiosRequestConfig<any> | undefined)
    : Promise<AxiosResponse> => axios.delete(url, config),
    post: async (url: string, data?: any, config?: AxiosRequestConfig<any> | undefined)
    : Promise<AxiosResponse> => axios.post(url, data, config),
    put: async (url: string, data?: any, config?: AxiosRequestConfig<any> | undefined)
    : Promise<AxiosResponse> => axios.put(url, data, config),
    patch: async (url: string, data?: any, config?: AxiosRequestConfig<any> | undefined)
    : Promise<AxiosResponse> => axios.patch(url, data, config),
  };
};

export default useAxios;
