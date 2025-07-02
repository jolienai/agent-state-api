SET search_path TO public;
    
-- Create enum for agent state if you want (or use VARCHAR)
-- CREATE TYPE agent_state_enum AS ENUM ('OnCall', 'OnLunch', 'Unknown');

-- Table: agents
CREATE TABLE agents (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    state TEXT NOT NULL, -- stored as string!
    created_at TIMESTAMPTZ NOT NULL,
    created_by TEXT NOT NULL,
    updated_at TIMESTAMPTZ,
    updated_by TEXT
);

-- Table: agent_skills
CREATE TABLE agent_skills (
  id TEXT PRIMARY KEY,
  agent_id TEXT NOT NULL,
  queue_id TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL,
  created_by TEXT NOT NULL,
  updated_at TIMESTAMPTZ,
  updated_by TEXT,
  CONSTRAINT fk_agent FOREIGN KEY (agent_id) REFERENCES agents(id) ON DELETE CASCADE
);

CREATE INDEX idx_agent_skills_agent_id ON agent_skills(agent_id);
CREATE INDEX idx_agent_skills_queue_id ON agent_skills(queue_id);