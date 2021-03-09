import styled from '@emotion/styled';
import React from 'react';
import { getState } from '../../state';
import ServerCard from './serverCard';

const NoServerMessage = styled.div`
  margin-top: 40px;
  text-align: center;
`;

const DashboardContainer = styled.div`
  margin:5px;
  display: flex;
  flex-wrap: wrap;

  * {
    margin-right: 10px;
  }

  @media (max-width: 600px){
    flex-wrap: nowrap;
    flex-direction: row;
  }
`;

const Dashboard: React.FunctionComponent = () => {
  const { state: { servers } } = getState();

  if (!servers.length)
    return (
      <NoServerMessage>
          <h1>No servers created yet...</h1>
          <h2>why don't you login and create the first one?</h2>
      </NoServerMessage>
    );

  return (
    <DashboardContainer>
      {servers.map(s => <ServerCard server={s} key={s.id} />)}
    </DashboardContainer>
  );
};

export default Dashboard;