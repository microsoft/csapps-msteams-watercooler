// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { devBaseUrl } from './environment/development';
import { localBaseUrl } from './environment/local';

export const getBaseUrl = (): string => {
    return devBaseUrl;
}

export const graphAPIUrl = 'https://graph.microsoft.com/v1.0';
