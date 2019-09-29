/**
 * @license XXX
 * (c) 2019 Bugfire https://bugfire.dev/
 * License: MIT
 */

import * as fs from "fs";

import { LoadConfig as LC, ConfigType } from "./common/config";

interface MyConfig {
}

const MyConfigType: ConfigType = {
};

export const LoadConfig = (filename: string): MyConfig => {
  return LC<MyConfig>(fs.readFileSync(filename, "utf8"), MyConfigType);
};
