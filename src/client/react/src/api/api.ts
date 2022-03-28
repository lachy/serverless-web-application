import axios, { AxiosRequestConfig } from 'axios';
import { useAuth0 } from '@auth0/auth0-react';

const apiUrl = process.env.REACT_APP_API_URL as string;

const client = axios.create({
  baseURL: apiUrl,
});

client.interceptors.request.use(
  async (config: AxiosRequestConfig) => {
    const { getAccessTokenSilently } = useAuth0();
    const token = await getAccessTokenSilently();
    return {
      ...config,
      headers: { ...config.headers, Authorization: `Bearer ${token}` },
    };
  },
  (error) => error,
);

export default async function getTodos() {
  const response = await client.get('todos');
  return response.data;
}
