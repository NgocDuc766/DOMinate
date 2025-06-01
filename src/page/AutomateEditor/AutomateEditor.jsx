import React, { useState } from 'react';

const BlockTypes = {
  VAR: 'var',
  MATH: 'math',
};

const BlockContainer = ({ id, blocks, setBlocks }) => {
  const addBlock = (type) => {
    const newId = Math.random().toString(36).substring(2, 9);
    const newBlock = {
      id: newId,
      type,
      name: type === BlockTypes.VAR ? '' : undefined,
      value: type === BlockTypes.VAR ? 0 : undefined,
      operator: '+',
      inputs: [],
    };
    setBlocks(prev =>
      prev.map(container =>
        container.id === id
          ? { ...container, children: [...container.children, newBlock] }
          : container
      )
    );
  };

  const updateChild = (childId, updates) => {
    setBlocks(prev =>
      prev.map(container =>
        container.id === id
          ? {
              ...container,
              children: container.children.map(b =>
                b.id === childId ? { ...b, ...updates } : b
              ),
            }
          : container
      )
    );
  };

  const renderChild = (block) => {
    if (block.type === BlockTypes.VAR) {
      return (
        <div style={styles.block}>
          <strong style={{ color: 'red' }}>Var</strong>:&nbsp;
          <input
            type="text"
            placeholder="Variable name"
            value={block.name}
            onChange={e => updateChild(block.id, { name: e.target.value })}
            style={{ width: '100px', marginRight: '10px' }}
          />
          <input
            type="number"
            value={block.value}
            onChange={e => updateChild(block.id, { value: e.target.value })}
            style={{ width: '80px' }}
          />
        </div>
      );
    }

    if (block.type === BlockTypes.MATH) {
      const container = blocks.find(b => b.id === id);
      return (
        <div style={styles.block}>
          <strong style={{ color: 'red' }}>Math</strong>:&nbsp;
          <select
            value={block.operator}
            onChange={(e) => updateChild(block.id, { operator: e.target.value })}
          >
            <option value="+">+</option>
            <option value="-">-</option>
            <option value="*">*</option>
            <option value="/">/</option>
          </select>
          &nbsp;
          <select
            value={block.inputs?.[0] || ''}
            onChange={(e) =>
              updateChild(block.id, { inputs: [e.target.value, block.inputs?.[1]] })
            }
          >
            <option value="">A block</option>
            {container?.children
              .filter(b => b.id !== block.id)
              .map(b => (
                <option key={b.id} value={b.id}>
                  {b.name ? `${b.name} (#${b.id.slice(0, 4)})` : `#${b.id.slice(0, 4)}`}
                </option>
              ))}
          </select>
          &nbsp;
          <select
            value={block.inputs?.[1] || ''}
            onChange={(e) =>
              updateChild(block.id, { inputs: [block.inputs?.[0], e.target.value] })
            }
          >
            <option value="">B block</option>
            {container?.children
              .filter(b => b.id !== block.id)
              .map(b => (
                <option key={b.id} value={b.id}>
                  {b.name ? `${b.name} (#${b.id.slice(0, 4)})` : `#${b.id.slice(0, 4)}`}
                </option>
              ))}
          </select>
        </div>
      );
    }
  };

  const current = blocks.find(b => b.id === id);

  return (
    <div style={styles.container}>
      <div>
        <strong style={{ color: 'red' }}>Block Container #{id.slice(0, 4)}</strong>
      </div>
      <button onClick={() => addBlock(BlockTypes.VAR)}>+ Var</button>
      <button onClick={() => addBlock(BlockTypes.MATH)}>+ Math</button>
      <div style={{ marginTop: 10 }}>
        {current?.children.map(block => (
          <div key={block.id}>{renderChild(block)}</div>
        ))}
      </div>
    </div>
  );
};

const AutomateEditor = () => {
  const [blocks, setBlocks] = useState([]);
  const [selectedMathId, setSelectedMathId] = useState('');
  const [result, setResult] = useState(undefined);

  const createContainer = () => {
    const id = Math.random().toString(36).substring(2, 9);
    setBlocks(prev => [...prev, { id, children: [] }]);
  };

  const getChildValue = (blockId) => {
    let targetBlock;
    blocks.forEach(container => {
      const found = container.children.find(b => b.id === blockId);
      if (found) targetBlock = found;
    });
    if (!targetBlock) return 0;

    if (targetBlock.type === BlockTypes.VAR) return Number(targetBlock.value) || 0;

    if (targetBlock.type === BlockTypes.MATH) {
      const [aId, bId] = targetBlock.inputs || [];
      const a = getChildValue(aId);
      const b = getChildValue(bId);
      switch (targetBlock.operator) {
        case '+': return a + b;
        case '-': return a - b;
        case '*': return a * b;
        case '/': return b !== 0 ? a / b : 0;
        default: return 0;
      }
    }

    return 0;
  };

  const executeMath = () => {
    if (!selectedMathId) return;
    const val = getChildValue(selectedMathId);
    setResult(val);
  };

  const allMathBlocks = blocks
    .flatMap(container => container.children)
    .filter(b => b.type === BlockTypes.MATH);

  return (
    <div>
      <button onClick={createContainer}>+ Create Block Container</button>
      <hr />
      {blocks.map(container => (
        <BlockContainer
          key={container.id}
          id={container.id}
          blocks={blocks}
          setBlocks={setBlocks}
        />
      ))}
      <hr />
      <div style={styles.container}>
        <strong style={{ color: 'red' }}>Execute Math</strong>:&nbsp;
        <select
          value={selectedMathId}
          onChange={(e) => setSelectedMathId(e.target.value)}
        >
          <option value="">
            {allMathBlocks.length === 0
              ? 'Không có phép toán để thực thi'
              : 'Chọn phép toán'}
          </option>
          {allMathBlocks.map(b => (
            <option key={b.id} value={b.id}>
              {`Math (#${b.id.slice(0, 4)})`}
            </option>
          ))}
        </select>
        <button onClick={executeMath} disabled={!selectedMathId} style={{ marginLeft: 8 }}>
          ▶ Thực thi
        </button>
        {result !== undefined && (
          <div style={{ marginTop: 10 }}>
            Kết quả: <strong style={{ color: 'red' }}>{result}</strong>
          </div>
        )}
      </div>
    </div>
  );
};

const styles = {
  container: {
    padding: '16px',
    marginBottom: '24px',
    border: '2px dashed #bbb',
    borderRadius: '10px',
    background: '#eef6ff',
    boxShadow: '0 2px 6px rgba(0,0,0,0.05)',
  },
  block: {
    padding: '12px',
    marginTop: '12px',
    border: '1px solid #ccc',
    borderRadius: '8px',
    background: '#ffffff',
    boxShadow: '0 1px 3px rgba(0,0,0,0.08)',
    fontFamily: 'Arial, sans-serif',
  },
};

export default AutomateEditor;
