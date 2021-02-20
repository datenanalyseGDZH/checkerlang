/*  Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

function hashCode(value) {
    const str = value;
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
      const chr = str.charCodeAt(i);
      hash = ((hash << 5) - hash) + chr;
      hash |= 0; // Convert to 32bit integer
    }
    return Math.abs(hash);
}

function valuesEquals(valuea, valueb) {
    return valuea.isEquals(valueb);
}

function valuesCompare(valuea, valueb) {
    return valuea.compareTo(valueb);
}

function hashTableSize(itemCount) {
    const prime_sizes = [17, 31, 61, 127, 251, 503, 1009, 2011, 4099, 8191, 16363, 33049, 68147];
    let min_size = itemCount * 1.3;
    for (const size of prime_sizes) {
        if (min_size < size) return size;
    }
    // if we exceed the list of predefined sizes, just use a multiple of the last value
    return Math.ceil(min_size / prime_sizes[prime_sizes.length - 1]) * prime_sizes[prime_sizes.length - 1];
}

export class HashSet {
    constructor(equalityFn = valuesEquals, compareFn = valuesCompare, hashFn = hashCode) {
        this.equalityFn = equalityFn;
        this.compareFn = compareFn;
        this.hashFn = hashFn;
        this.length = 0;
        this.size = 0;
        this.table_size = hashTableSize(this.size);
        this.table = new Array(this.table_size);
    }

    _get_index(item) {
        return Math.abs(this.hashFn(item.toString())) % this.table_size;
    }

    _rehash() {
        const old_table = this.table;
        this.table_size = hashTableSize(this.size);
        this.table = new Array(this.table_size);
        this.size = 0;
        for (const list of old_table) {
            if (list !== undefined) {
                for (const entry of list) {
                    this._add_item(entry, this._get_index(entry));
                }
            }
        }
    }

    _add_item(item, hash) {
        if (this.table[hash] === undefined) {
            this.table[hash] = [item];
            this.size++;
        } else {
            for (const entry of this.table[hash]) {
                if (this.equalityFn(entry, item)) return;
            }
            this.table[hash].push(item);
            this.size++;
        }
    }

    add(item) {
        this._add_item(item, this._get_index(item));
        if (this.table_size < this.size) this._rehash();
        return this;
    }

    has(item) {
        const list = this.table[this._get_index(item)];
        if (list !== undefined) {
            for (const entry of list) {
                if (this.equalityFn(entry, item)) return true;
            }
        }
        return false;
    }

    remove(item) {
        const idx = this._get_index(item);
        const list = this.table[idx];
        if (list !== undefined) {
            for (let i = 0; i < list.length; i++) {
                if (this.equalityFn(list[i], item)) {
                    list.splice(i, 1);
                    this.size--;
                    break;
                }
            }
            if (list.length === 0) this.table[idx] = undefined;
        }
        if (this.size < this.table_size / 2) this._rehash();
        return this;
    }

    values() {
        let result = [];
        for (const list of this.table) {
            if (list !== undefined) {
                result = result.concat(list);
            }
        }
        return result;
    }

    sortedValues() {
        const result = this.values();
        result.sort(this.compareFn);
        return result;
    }

    toString() {
        let result = "HashSet([";
        for (const item of this.sortedValues()) {
            result = result.concat(item.toString(), ", ");
        }
        if (result.endsWith(", ")) result = result.substr(0, result.length - 2);
        return result + "])";
    }
}

export class HashMap {
    constructor(equalityFn = valuesEquals, compareFn = valuesCompare, hashFn = hashCode) {
        this.equalityFn = equalityFn;
        this.compareFn = compareFn;
        this.hashFn = hashFn;
        this.length = 0;
        this.size = 0;
        this.table_size = hashTableSize(this.size);
        this.table = new Array(this.table_size);
    }

    _get_index(key) {
        return Math.abs(this.hashFn(key.toString())) % this.table_size;
    }

    _rehash() {
        const old_table = this.table;
        this.table_size = hashTableSize(this.size);
        this.table = new Array(this.table_size);
        this.size = 0;
        for (const list of old_table) {
            if (list !== undefined) {
                for (const [key, value] of list) {
                    this._add_item(key, value, this._get_index(key));
                }
            }
        }
    }

    _add_item(key, value, hash) {
        if (this.table[hash] === undefined) {
            this.table[hash] = [[key, value]];
            this.size++;
        } else {
            for (const entry of this.table[hash]) {
                if (this.equalityFn(entry[0], key)) {
                    entry[1] = value;
                    return;
                }
            }
            this.table[hash].push([key, value]);
            this.size++;
        }
    }

    set(key, value) {
        this._add_item(key, value, this._get_index(key));
        if (this.table_size < this.size) this._rehash();
        return this;
    }

    get(key) {
        const list = this.table[this._get_index(key)];
        if (list !== undefined) {
            for (const entry of list) {
                if (this.equalityFn(entry[0], key)) return entry[1];
            }
        }
        return null;
    }

    has(key) {
        const list = this.table[this._get_index(key)];
        if (list !== undefined) {
            for (const entry of list) {
                if (this.equalityFn(entry[0], key)) return true;
            }
        }
        return false;
    }

    remove(key) {
        const idx = this._get_index(key);
        const list = this.table[idx];
        if (list !== undefined) {
            for (let i = 0; i < list.length; i++) {
                if (this.equalityFn(list[i][0], key)) {
                    list.splice(i, 1);
                    this.size--;
                    break;
                }
            }
            if (list.length === 0) this.table[idx] = undefined;
        }
        if (this.size < this.table_size / 2) this._rehash();
        return this;
    }

    keys() {
        let result = [];
        for (const list of this.table) {
            if (list !== undefined) {
                for (const [key, value] of list) {
                    result.push(key);
                }
            }
        }
        return result;
    }

    sortedKeys() {
        const result = this.keys();
        result.sort(this.compareFn);
        return result;
    }

    values() {
        let result = [];
        for (const list of this.table) {
            if (list !== undefined) {
                for (const [key, value] of list) {
                    result.push(value);
                }
            }
        }
        return result;
    }

    sortedValues() {
        const result = this.values();
        result.sort(this.compareFn);
        return result;
    }

    entries() {
        let result = [];
        for (const list of this.table) {
            if (list !== undefined) {
                result = result.concat(list)
            }
        }
        return result;
    }

    sortedEntries() {
        const result = this.entries();
        result.sort((a, b) => this.compareFn(a[0], b[0]));
        return result;
    }

    toString() {
        let result = "HashMap([";
        for (const [key, value] of this.sortedEntries()) {
            result = result.concat(key.toString(), " => ", value.toString(), ", ");
        }
        if (result.endsWith(", ")) result = result.substr(0, result.length - 2);
        return result + "])";
    }
}
