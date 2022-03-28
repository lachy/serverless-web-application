import * as React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router } from 'react-router-dom';
import { createStore, compose } from 'redux';
import { Provider } from 'react-redux';
import CssBaseline from '@mui/material/CssBaseline';
import { ThemeProvider } from '@mui/material/styles';
import Auth0ProviderWithHistory from './auth/auth0-provider-with-history';
import App from './App';
import theme from './theme';
import reducer from './reducers';
import middleware from './middleware';

const store = createStore(
  reducer,
  compose(middleware),
);

ReactDOM.render(
  <Router>
    <Auth0ProviderWithHistory>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Provider store={store}>
          <App />
        </Provider>
      </ThemeProvider>
    </Auth0ProviderWithHistory>
  </Router>,
  document.getElementById('root'),
);
