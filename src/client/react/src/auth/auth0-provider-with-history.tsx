import React, { ReactNode } from 'react';
import { useHistory } from 'react-router-dom';
import { AppState, Auth0Provider } from '@auth0/auth0-react';

type Auth0ProviderWithHistoryProps = {
  children: ReactNode
};

const Auth0ProviderWithHistory = ({ children }: Auth0ProviderWithHistoryProps) => {
  const history = useHistory();
  const domain = process.env.REACT_APP_AUTH0_DOMAIN as string;
  const clientId = process.env.REACT_APP_AUTH0_CLIENT_ID as string;
  const audience = process.env.REACT_APP_AUTH0_AUDIENCE as string;

  const onRedirectCallback = (appState: AppState) => {
    history.push(appState?.returnTo || window.location.pathname);
  };

  return (
    <Auth0Provider
      domain={domain}
      clientId={clientId}
      redirectUri={window.location.origin}
      onRedirectCallback={onRedirectCallback}
      audience={audience}
    >
      {children}
    </Auth0Provider>
  );
};

export default Auth0ProviderWithHistory;
