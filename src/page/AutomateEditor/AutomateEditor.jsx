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
  return (
    <div style={styles.block}>
      <strong style={{ color: 'red' }}>Math</strong>:&nbsp;
      <input
        type="text"
        placeholder="Result variable name"
        value={block.name || ''}
        onChange={e => updateChild(block.id, { name: e.target.value })}
        style={{ width: '100px', marginRight: '10px' }}
      />
      <input
        type="text"
        placeholder="Expression (e.g. a+b)"
        value={block.expression || ''}
        onChange={e => updateChild(block.id, { expression: e.target.value })}
        style={{ width: '200px' }}
      />
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

  const executeMath = () => {
  if (!selectedMathId) return;

  const target = blocks
    .flatMap(container => container.children)
    .find(b => b.id === selectedMathId);

  if (!target || !target.expression || !target.name) return;

  // Biến env: chứa tất cả biến kiểu VAR
  const env = {};
  blocks.forEach(container => {
    container.children.forEach(b => {
      if (b.type === BlockTypes.VAR && b.name) {
        env[b.name] = Number(b.value) || 0;
      }
    });
  });

  try {
    const expr = target.expression.replace(/[^\w\d+\-*/(). ]/g, '');
    const func = new Function(...Object.keys(env), `return ${expr};`);
    const val = func(...Object.values(env));

    // Gán kết quả cho biến mới hoặc cập nhật
    const newBlocks = [...blocks];
    let found = false;
    for (const container of newBlocks) {
      for (const b of container.children) {
        if (b.type === BlockTypes.VAR && b.name === target.name) {
          b.value = val;
          found = true;
        }
      }
    }

    // Nếu chưa có biến với name đó → tạo mới
    if (!found) {
      const newVar = {
        id: Math.random().toString(36).substring(2, 9),
        type: BlockTypes.VAR,
        name: target.name,
        value: val,
        operator: '+',
        inputs: [],
      };
      newBlocks[0]?.children.push(newVar); // thêm vào container đầu tiên
    }

    setBlocks(newBlocks);
    setResult(val);
  } catch (e) {
    alert('Biểu thức không hợp lệ!');
  }
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
