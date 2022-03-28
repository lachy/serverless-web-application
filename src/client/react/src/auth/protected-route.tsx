import React, { ReactNode } from 'react';
import { Route, RouteProps } from 'react-router-dom';
import { withAuthenticationRequired } from '@auth0/auth0-react';

const loadingImg = 'https://cdn.auth0.com/blog/auth0-react-sample/assets/loading.svg';

const Loading = () => (
  <div className="spinner">
    <img src={loadingImg} alt="Loading..." />
  </div>
);

interface ProtectedRouteProps extends RouteProps {
  component: React.ComponentType;
}

/* eslint-disable react/jsx-props-no-spreading */
const ProtectedRoute = ({ component, ...args }: ProtectedRouteProps) => (
  <Route
    component={withAuthenticationRequired(component, {
      onRedirecting: () => <Loading />,
    })}
    {...args}
  />
);

export default ProtectedRoute;
