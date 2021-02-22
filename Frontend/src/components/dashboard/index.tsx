import styled from '@emotion/styled';
import React from 'react';
import { getState } from '../../state';
import ServerCard from './serverCard';

const CenterMessage = styled.div`
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
`;

const DashboardContainer = styled.div`
  margin:5px;
  display: flex;
  flex-wrap: wrap;

  * {
    margin-right: 10px;
  }
`;

const Dashboard: React.FunctionComponent = () => {
  const { state: { servers } } = getState();

  if (!servers.length)
    return (
      <CenterMessage>
        <div>
          <h1>No servers created yet...</h1>
          <h2>why don't you login and create the first one?</h2>
        </div>
      </CenterMessage>
    );

  return (
    <DashboardContainer>
      {servers.map(s => <ServerCard server={s} key={s.id} />)}
    </DashboardContainer>
  );
};

export default Dashboard;